using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Services
{
    using System.Net.Mime;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Common;
    using Data;
    using Exceptions;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Models.RequestModels;
    using Models.ResponseModels;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class CaseService
    {
        private readonly AppDbContext context; // use repo
        private readonly IMapper mapper;
        private readonly UsersService usersService;
        private readonly CurrentUserService currentUserService;

        public CaseService(AppDbContext context, IMapper mapper, UsersService usersService,
            CurrentUserService currentUserService)
        {
            this.context = context;
            this.mapper = mapper;
            this.usersService = usersService;
            this.currentUserService = currentUserService;
        }

        public IEnumerable<CodeMapModel> GetTreeModel(Guid? id)
        {
            var result = context.Codes.AsQueryable();
            if (id == null)
            {
                result = result.Where(x => x.Level == 2);
            }
            else
            {
                result = result.Where(x => x.Parents.Select(p => p.ParentId).Contains(id.Value));
            }

            return result.ProjectTo<CodeMapModel>(mapper.ConfigurationProvider);
        }

        internal async Task<SuggestionsModel> GetSuggestions(string code)
        {
            var suggestions = await this.context.Jurisdictions.Where(x => x.JurCode == code) // optimize this?
                .Select(j => new SuggestionsModel
                {
                    Courts = j.Courts.Select(x => x.Name),
                    CourtsEng = j.CourtsEng.Select(x => x.Name),
                    Sources = j.Sources.Select(x => x.Name)
                }).FirstOrDefaultAsync();
            suggestions.Keywords = await this.context.Keywords.Select(x => x.Name).ToListAsync();
            return suggestions;
        }

        internal async Task DeleteSuggestion(SuggestionReqModel model) // fix this shit
        {
            if (model.Type == "Court")
            {
                var sug = this.context.Court.FirstOrDefault(x =>
                    x.Name == model.Name && x.Jurisdiction.JurCode == model.JurCode);
                if (sug != null)
                {
                    this.context.Court.Remove(sug);
                }
            }
            else if (model.Type == "CourtEng")
            {
                var sug = this.context.CourtEng.FirstOrDefault(x =>
                    x.Name == model.Name && x.Jurisdiction.JurCode == model.JurCode);
                if (sug != null)
                {
                    this.context.CourtEng.Remove(sug);
                }
            }
            else if (model.Type == "Source")
            {
                var sug = this.context.Source.FirstOrDefault(x =>
                    x.Name == model.Name && x.Jurisdiction.JurCode == model.JurCode);
                if (sug != null)
                {
                    this.context.Source.Remove(sug);
                }
            }
            else if (model.Type == "Keywords")
            {
                var sug = new Keyword {Name = model.Name};
                this.context.Remove(sug);
            }
            else
            {
                throw new NotFoundException("Type not found!");
            }

            await this.context.SaveChangesAsync();
        }

        internal IReadOnlyList<AllSuggestionsModel> GetAllSuggestions()
        {
            var court = this.context.Court.Select(x => new {x.Name, x.Jurisdiction.JurCode})
                .AsEnumerable()
                .GroupBy(x => x.JurCode, x => x.Name).ToDictionary(x => x.Key, x => x.ToList());
            var courtEng = this.context.CourtEng.Select(x => new {x.Name, x.Jurisdiction.JurCode})
                .AsEnumerable()
                .GroupBy(x => x.JurCode, x => x.Name).ToDictionary(x => x.Key, x => x.ToList());
            var source = this.context.Source.Select(x => new {x.Name, x.Jurisdiction.JurCode})
                .AsEnumerable()
                .GroupBy(x => x.JurCode, x => x.Name).ToDictionary(x => x.Key, x => x.ToList());
            var list = new List<AllSuggestionsModel>();
            var keys = court.Keys.Union(courtEng.Keys).Union(source.Keys);
            foreach (var key in keys)
            {
                list.Add(new AllSuggestionsModel
                {
                    JurisdictionCode = key,
                    Courts = court.ContainsKey(key) ? court[key] : Enumerable.Empty<string>(),
                    CourtsEng = courtEng.ContainsKey(key) ? courtEng[key] : Enumerable.Empty<string>(),
                    Sources = source.ContainsKey(key) ? source[key] : Enumerable.Empty<string>()
                });
            }

            var keywordsItem = new AllSuggestionsModel
            {
                JurisdictionCode = "Keywords",
                Keywords = this.context.Keywords.Select(x => x.Name).ToList()
            };
            if (keywordsItem.Keywords.Count() > 0)
            {
                list.Add(keywordsItem);
            }

            return list;
        }

        public IReadOnlyList<TreeNode> GetWholeTree()
        {
            var codes = context.Codes.AsNoTracking()
                .Select(c => new {c.Id, c.Code, c.Level, c.Texts.FirstOrDefault().Text})
                .ToDictionary(c => c.Id);
            var relationships = context.Relationships.AsNoTracking().AsEnumerable().GroupBy(rel => rel.ParentId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ChildId));
            var list = new List<TreeNode>();
            foreach (var mainNode in codes.Values.Where(x => x.Level == 2))
            {
                var node = new TreeNode
                {
                    Id = mainNode.Id,
                    Data = mainNode.Code,
                    Label = mainNode.Text,
                    Selectable = false
                };
                list.Add(node);
                FillChildren(node);
            }

            return list;

            void FillChildren(TreeNode node)
            {
                var hasChildren = relationships.TryGetValue(node.Id, out var childrenIds);
                if (hasChildren)
                {
                    foreach (var childId in childrenIds)
                    {
                        var data = codes[childId];
                        var newNode = new TreeNode
                        {
                            Id = data.Id,
                            Data = data.Code,
                            Label = data.Text,
                            Selectable = data.Level > 3
                        };
                        node.Children.Add(newNode);
                        FillChildren(newNode);
                    }
                }
            }
        }

        internal async Task DeleteCase(int id)
        {
            var record = await this.context.Cases.Where(c => c.Id == id).Select(c => new
            {
                c.Id,
                c.UserId,
                OrgName = c.User.Organization.ShortName,
                c.IsDeleted
            }).FirstOrDefaultAsync();
            if (record == null)
            {
                throw new NotFoundException($"Case with id {id} not found");
            }

            var hasRightToDelete = this.HasEditDeleteRights(record.UserId, record.OrgName);


            if (hasRightToDelete)
            {
                var caseEntity = new Case {Id = record.Id};
                context.Cases.Attach(caseEntity);
                caseEntity.IsDeleted = true;
                await context.SaveChangesAsync();
            }
            else
            {
                throw new NotAuthorizedException();
            }
        }

        public async Task DeleteMeta(int id)
        {
            var record = await this.context.Metadata.Where(c => c.Id == id).Select(c => new
            {
                c.Id,
                c.UserId,
                OrgName = c.User.Organization.ShortName,
                c.IsDeleted
            }).FirstOrDefaultAsync();
            if (record == null)
            {
                throw new NotFoundException($"Metadata with id {id} not found");
            }

            var hasRightToDelete = this.HasEditDeleteRights(record.UserId, record.OrgName);


            if (hasRightToDelete)
            {
                var metaEntity = new Metadata {Id = record.Id};
                this.context.Metadata.Attach(metaEntity);
                metaEntity.IsDeleted = true;
                await this.context.SaveChangesAsync();
            }
            else
            {
                throw new NotAuthorizedException();
            }
        }

        public async Task<object> GetMetadataList(CaseListRequestModel request)
        {
            var result = await this.GetCaseList(request, true);
            return result;
        }

        private bool HasEditDeleteRights(string userId, string orgName)
        {
            var isSuperAdmin = this.currentUserService.IsSuperAdmin();
            var isAdmin = this.currentUserService.IsAdmin();
            var hasRightToDelete = false;
            if (isSuperAdmin)
            {
                hasRightToDelete = true;
            }
            else if (isAdmin)
            {
                var orgClaim = this.currentUserService.GetCurrentUserOrgClaim();
                if (orgClaim == orgName)
                {
                    hasRightToDelete = true;
                }
            }
            else
            {
                var idClaim = this.currentUserService.GetCurrentUserIdClaim();
                if (idClaim == userId)
                {
                    hasRightToDelete = true;
                }
            }

            return hasRightToDelete;
        }

        public async Task<IEnumerable<CaseListResponseModel>> GetCaseList(CaseListRequestModel request,
            bool isMeta = false)
        {
            var isSuper = this.currentUserService.IsSuperAdmin();
            var isAdmin = this.currentUserService.IsAdmin();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            IQueryable<IMetaCase> query;
            if (isMeta)
            {
                query = this.context.Metadata.Include(x => x.User).ThenInclude(x => x.Organization)
                    .Where(c => c.IsDeleted == false);
            }
            else
            {
                query = this.context.Cases.Include(x => x.User).ThenInclude(x => x.Organization)
                    .Where(c => c.IsDeleted == false);
            }

            if (request.UserName.IsNotNull() && isAdmin)
            {
                query = query.Where(x => EF.Functions.Like(x.User.UserName, $"%{request.UserName}%"));
            }

            //            else
            //            {
            //                if (!isAdmin)
            //                {
            //                    var userId = this.currentUserService.GetCurrentUserIdClaim();
            //                    query = query.Where(x => x.UserId == userId);
            //                }
            //            }
            //
            if (request.Organization.IsNotNull() && isSuper)
            {
                query = query.Where(x => x.User.Organization.ShortName == request.Organization);
            }
            //            else if (!isSuper)
            //            {
            //                var orgClaim = this.currentUserService.GetCurrentUserOrgClaim();
            //                query = query.Where(x => x.User.Organization.ShortName == orgClaim);
            //            }


            var tempRes = (from item in query
                let content = JObject.Parse(item.Content)
                select new
                {
                    item.Id,
                    item.LastChange,
                    item.Caption,
                    item.User.UserName,
                    Editable = this.HasEditDeleteRights(item.UserId, item.User.Organization.ShortName),
                    OrgShortName = item.User.Organization.ShortName,
                    Content = content,
                    DocDate = content["dateOfDocument"]
                }).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.JurisdictionCode))
            {
                tempRes = tempRes.Where(x =>
                {
                    var juri = x.Content["jurisdiction"];
                    if (juri != null && juri.Type != JTokenType.Null)
                    {
                        var jurisdictionCode = (string) juri["code"];
                        if (jurisdictionCode == request.JurisdictionCode)
                        {
                            return true;
                        }
                    }

                    return false;
                });
            }


            var result = tempRes.OrderByDescending(x =>
                {
                    if (x.DocDate != null && x.DocDate.Type != JTokenType.Null)
                    {
                        return (DateTime) x.DocDate; // trusting JsonConvert for now >_>
                    }

                    return x.LastChange;
                })
                .Select(x => new CaseListResponseModel
                {
                    CaseId = x.Id,
                    LastChange = x.LastChange,
                    Title = x.Caption,
                    UserName = x.UserName,
                    Editable = x.Editable,
                    Organization = x.OrgShortName,
                    DocDate = (x.DocDate != null && x.DocDate.Type != JTokenType.Null)
                        ? (DateTime) x.DocDate
                        : (DateTime?) null
                });


            return result.ToList();
        }

        public async Task<CaseMetaContentResponseModel> GetCaseContent(int caseId)
        {
            var dbInfo = await this.context.Cases.AsNoTracking().Where(x => x.Id == caseId)
                .Select(x => new
                {
                    x.Content,
                    x.UserId,
                    OrgShortName = x.User.Organization.ShortName
                }).SingleAsync();
            var editable = this.HasEditDeleteRights(dbInfo.UserId, dbInfo.OrgShortName);
            return new CaseMetaContentResponseModel
            {
                Content = dbInfo.Content,
                Editable = editable
            };
        }

        public async Task<CaseMetaContentResponseModel> GetMetaContent(int id)
        {
            var dbInfo = await this.context.Metadata.AsNoTracking().Where(x => x.Id == id).Select(x => new
            {
                x.Content,
                x.UserId,
                OrgShortName = x.User.Organization.ShortName
            }).SingleAsync();
            var editable = this.HasEditDeleteRights(dbInfo.UserId, dbInfo.OrgShortName);
            return new CaseMetaContentResponseModel
            {
                Content = dbInfo.Content,
                Editable = editable
            };
        }

        public async Task<int> SaveCase(CaseModel model)
        {
            var caseEntity = this.mapper.Map<Case>(model);
            caseEntity.LastChange = DateTime.Now;
            caseEntity.User = await this.usersService.GetCurrentApplicationUser();
            var caseInserted = this.context.Cases.Add(caseEntity);
            this.SaveAutoCompleteSuggestions(model);
            await this.context.SaveChangesAsync();
            return caseInserted.Entity.Id;
        }

        public async Task<int> SaveMetadata(MetadataModel model)
        {
            var metaEntity = new Metadata
            {
                Caption = model.Title,
                LastChange = DateTime.Now,
                User = await this.usersService.GetCurrentApplicationUser(),
                Content = model.Content,
            };
            if (model.File != null)
            {
                var file = new MetaFile
                {
                    Name = model.File.Filename,
                    MimeType = model.File.MimeType,
                    Content = Convert.FromBase64String(model.File.Base64Content)
                };
                metaEntity.MetaFile.Add(file);
            }

            if (model.TranslatedFile != null)
            {
                var file = new MetaTranslatedFile
                {
                    Name = model.TranslatedFile.Filename,
                    MimeType = model.TranslatedFile.MimeType,
                    Content = Convert.FromBase64String(model.TranslatedFile.Base64Content)
                };
                metaEntity.MetaTranslatedFile.Add(file);
            }

            var metaInserted = this.context.Metadata.Add(metaEntity); // are there gonna be suggestions here??
            await this.context.SaveChangesAsync();
            return metaInserted.Entity.Id;
        }

        public async Task EditCase(CaseModel model, int id)
        {
            var caseInfo = await this.context.Cases.Where(x => x.Id == id).Select(x => new
            {
                Case = x,
                OrgShortName = x.User.Organization.ShortName,
                UserId = x.UserId
            }).SingleAsync();
            var hasRightToEdit = this.HasEditDeleteRights(caseInfo.UserId, caseInfo.OrgShortName);
            if (!hasRightToEdit)
            {
                throw new NotAuthorizedException();
            }

            var oldRecord = caseInfo.Case;
            if (oldRecord == null)
            {
                throw new NotFoundException($"Case with id {id} not found");
            }

            oldRecord.Content = model.Content;
            oldRecord.LastChange = DateTime.Now;
            oldRecord.Caption = model.Title;


            var caseLog = this.mapper.Map<CaseLog>(oldRecord);
            caseLog.UserId = this.currentUserService.GetCurrentUserIdClaim();
            this.context.CasesLog.Add(caseLog);
            this.SaveAutoCompleteSuggestions(model);
            await this.context.SaveChangesAsync();
        }

        public async Task EditMetadata(MetadataModel model, int id)
        {
            var metaInfo = await this.context.Metadata.Where(x => x.Id == id).Select(x => new
            {
                Meta = x,
                OrgShortName = x.User.Organization.ShortName,
                x.UserId
            }).SingleAsync();

            var hasRightToEdit = this.HasEditDeleteRights(metaInfo.UserId, metaInfo.OrgShortName);
            if (!hasRightToEdit)
            {
                throw new NotAuthorizedException();
            }

            var oldRecord = metaInfo.Meta;
            if (oldRecord == null)
            {
                throw new NotFoundException($"Metadata with id {id} not found");
            }

            oldRecord.Content = model.Content;
            oldRecord.LastChange = DateTime.Now;
            oldRecord.Caption = model.Title;
            if (model.File != null && model.File.Base64Content != null)
            {
                var file = new MetaFile
                {
                    Name = model.File.Filename,
                    MimeType = model.File.MimeType,
                    Content = Convert.FromBase64String(model.File.Base64Content)
                };
                var oldFileId = this.context.MetaFile.Where(x => x.MetadataId == id).Select(x => x.Id).FirstOrDefault();
                if (oldFileId == 0)
                {
                    oldRecord.MetaFile.Add(file);
                }
                else
                {
                    file.Id = oldFileId;
                    file.MetadataId = id;
                    var entry = this.context.MetaFile.Attach(file);
                    entry.State = EntityState.Modified;
                }
            }
            else if (model.File == null)
            {
                var oldFileId = this.context.MetaFile.Where(x => x.MetadataId == id).Select(x => x.Id).FirstOrDefault();
                if (oldFileId != 0)
                {
                    var entry = this.context.MetaFile.Attach(new MetaFile {Id = oldFileId});
                    entry.State = EntityState.Deleted;
                }
            }

            if (model.TranslatedFile != null && model.TranslatedFile.Base64Content != null)
            {
                var file = new MetaTranslatedFile
                {
                    Name = model.TranslatedFile.Filename,
                    MimeType = model.TranslatedFile.MimeType,
                    Content = Convert.FromBase64String(model.TranslatedFile.Base64Content)
                };
                var oldFileId = this.context.MetaTranslatedFile.Where(x => x.MetadataId == id).Select(x => x.Id)
                    .FirstOrDefault();
                if (oldFileId == 0)
                {
                    oldRecord.MetaTranslatedFile.Add(file);
                }
                else
                {
                    file.Id = oldFileId;
                    file.MetadataId = id;
                    var entry = this.context.MetaTranslatedFile.Attach(file);
                    entry.State = EntityState.Modified;
                }
            }
            else if (model.TranslatedFile == null)
            {
                var oldFileId = this.context.MetaTranslatedFile.Where(x => x.MetadataId == id).Select(x => x.Id)
                    .FirstOrDefault();
                if (oldFileId != 0)
                {
                    var entry = this.context.MetaTranslatedFile.Attach(new MetaTranslatedFile {Id = oldFileId});
                    entry.State = EntityState.Deleted;
                }
            }

            var log = this.mapper.Map<MetadataLog>(oldRecord);
            log.UserId = this.currentUserService.GetCurrentUserIdClaim();
            this.context.MetadataLog.Add(log);
            await this.context.SaveChangesAsync();
        }

        private void SaveAutoCompleteSuggestions(CaseModel model)
        {
            if (string.IsNullOrEmpty(model.Content))
            {
                return;
            }

            var parsed = JsonConvert.DeserializeObject<CaseEditorDetailModel>(model.Content);
            if (string.IsNullOrEmpty(parsed.Jurisdiction?.Code))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(parsed.Court))
            {
                parsed.Court = null;
            }

            if (string.IsNullOrWhiteSpace(parsed.CourtEng))
            {
                parsed.CourtEng = null;
            }

            if (string.IsNullOrWhiteSpace(parsed.Source))
            {
                parsed.Source = null;
            }

            if (parsed.Keywords?.Length > 0)
            {
                var dbKeywords = this.context.Keywords.Select(x => x.Name).ToList();
                var entities = parsed.Keywords.Except(dbKeywords).Select(x => new Keyword {Name = x});
                this.context.Keywords.AddRange(entities); // context.save is in calling method
            }

            this.context.Database.ExecuteSqlCommand("suggest.InsertSuggestions @p0, @p1, @p2, @p3",
                parsed.Jurisdiction.Code, parsed.Court, parsed.CourtEng, parsed.Source);
        }

        public async Task<MetaFile> GetMetaFile(int id)
        {
            var data = await this.context.MetaFile.Where(x => x.MetadataId == id).FirstOrDefaultAsync();
            return data;
        }

        public async Task<MetaTranslatedFile> GetMetaTranslatedFile(int id)
        {
            var data = await this.context.MetaTranslatedFile.Where(x => x.MetadataId == id).FirstOrDefaultAsync();
            return data;
        }
    }
}
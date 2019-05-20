using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Automapper_Profiles
{
    using AutoMapper;
    using Data;
    using Models.RequestModels;
    using Models.ResponseModels;

    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<Codes, CodeMapModel>()
                .ForMember(x => x.Data, opt => opt.MapFrom(src => src.Id))
                .ForMember(x => x.Label,
                    opt => opt.MapFrom(src => src.Texts.Select(text => text.Text).FirstOrDefault()))
                .ForMember(x => x.Leaf, opt => opt.MapFrom(src => src.Children.Count == 0))
                .ForMember(x => x.Selectable, opt => opt.MapFrom(src => src.Level > 3));

            CreateMap<CaseModel, Case>()
                .ForMember(x => x.Caption, opt => opt.MapFrom(src => src.Title));
            //.ForMember(x => x.LastChange, opt => opt.MapFrom(src => src.Date));

            CreateMap<Case, CaseLog>()
                .ForMember(x => x.CaseId, opt => opt.MapFrom(src => src.Id))
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.User, opt => opt.Ignore())
                .ForMember(x => x.UserId, opt => opt.Ignore())
                .ForMember(x => x.ChangeDate, opt => opt.MapFrom(src => src.LastChange));
            // Add other maps here, or create additional Profiles - they are found automatically
        }
    }
}

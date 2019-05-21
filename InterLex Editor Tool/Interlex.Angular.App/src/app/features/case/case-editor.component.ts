import {Component, OnInit} from '@angular/core';
import {HttpService} from '../../core/services/http.service';
import {
  countryList,
  decisionTypes,
  ICaseEditor,
  IEubaseLink,
  IEucaseLink,
  IEulawLink,
  INomenclature,
  languageList,
  nationalities,
  yesNoRule,
  countries, NationalLawRecord, NationalCaseLawRecord, ICaseMetaContentResponse
} from '../../models/case-editor.model';
import {ActivatedRoute, Router} from '@angular/router';
import {AlertService} from '../../core/services/alert.service';
import {ConfirmationService, TreeNode} from 'primeng/api';
import {SuggestionsModel} from '../../models/suggestions.model';
import {OverlayPanel} from "primeng/primeng";
import {environment} from "../../../environments/environment";

@Component({
  selector: 'app-case-editor',
  templateUrl: 'case-editor.component.html',
  styleUrls: ['./case-editor.component.css'],
  providers: [ConfirmationService]
})
export class CaseEditorComponent implements OnInit {
  mod: ICaseEditor;
  test: string;
  showEuLink: boolean;
  showEuCaseLink: boolean;
  showAddNationalLaw: boolean;
  showAddNationalCaseLaw: boolean;
  showEuDoc: boolean;
  isEuDoc: boolean;
  isBgDoc: boolean;
  tmpEuDoc: IEucaseLink;
  previewHtmlContent: string;
  filteredNationalities: string[];
  filteredDomiciles: string[];
  nationalities: string[];
  filteredKeywords: string[];
  keywords: string[] = [];
  keywordCheckboxes: string[] = [];
  courtSuggestions: string[] = [];
  courtSuggestionsFiltered: string[];
  courtEngSuggestions: string[] = [];
  courtEngSuggestionsFiltered: string[];
  sourceSuggestions: string[] = [];
  sourceSuggestionsFiltered: string[];
  selectedTreeNodes: TreeNode[] = [];
  euroVocChips: string[] = null;
  showTree = false;
  editEuLinkIndex = -1;
  tmpEuLink: IEulawLink;
  editable = true;
  editEucaseIndex = -1;
  editNatLawIndex: number;
  editNatCaseLawIndex: number;
  tmpEucaseLink: IEucaseLink;
  environment = environment;
  href: string;
  countries: INomenclature[];
  languages: INomenclature[];
  decisionTypes: INomenclature[];
  countryLanguagesMap: Map<string, string>;
  caseId: string;

  keyword: string;
  nationality: string;
  domicileCountry: string;
  domicileRole: 'applicant' | 'defendant' = 'applicant';
  showKeyword: boolean;
  showAddNationality: boolean;
  showAddDomicile: boolean;

  submitted = false;

  yesNoRule: INomenclature[];
  linkHint = '';

  constructor(private http: HttpService, public router: Router, private route: ActivatedRoute, private alertService: AlertService,
              private confirmation: ConfirmationService) {
    this.countries = countryList;
    this.languages = languageList;
    this.decisionTypes = decisionTypes;
    this.initEmptyModel();
    this.route.paramMap.subscribe((params) => {
      this.caseId = params.get('id');
      if (this.caseId) {
        this.initModelFromApi();
      }
    });

    this.tmpEuLink = new IEulawLink();
    this.tmpEucaseLink = new IEucaseLink();
    this.tmpEuDoc = new IEucaseLink();
    this.yesNoRule = yesNoRule;

    this.showEuLink = false;
    this.showEuCaseLink = false;
    this.showKeyword = false;
    this.showEuDoc = false;
    this.isEuDoc = false;
  }

  initModelFromApi() {
    const id = this.caseId;
    this.editable = false;
    this.http.get('./case/GetCaseContent/' + this.caseId).subscribe((response: ICaseMetaContentResponse) => {
      let data = JSON.parse(response.content) as ICaseEditor;
      this.editable = response.editable;
      this.mod = {
        ...data,
        dateOfDocument: data.dateOfDocument === null ? null : new Date(data.dateOfDocument),
        nationalLawRecords: data.nationalLawRecords ? data.nationalLawRecords : [],
        nationalCaseLawRecords: data.nationalCaseLawRecords ? data.nationalCaseLawRecords : []
      };
      this.mapEuroVocToTreeAndChips(data.eurovoc);
      this.onModelChange(null);
      if (data.jurisdiction && data.jurisdiction.code) { // maybe pipe this with observable?
        this.getSuggestionsFromApi(data.jurisdiction.code);
      }
    }, (error) => {

    });
  }

  initEmptyModel() {
    this.mod = {
      title: '',
      court: '',
      courtEng: '',
      dateOfDocument: null,
      jurisdiction: null, // { label: '', code: '' },
      keywords: null,
      nationalities: null,
      domiciles: null,
      summary: '',
      language: null, // { label: '', code: '' },
      decisionType: null, // { label: '', code: '' },
      ecli: '',
      euCaselaw: [],
      euProvisions: [],
      nationalLawRecords: [],
      nationalCaseLawRecords: [],
      eurovoc: [],
      interlexOntology: '',
      internationalCaseLaw: '',
      internationalLaw: '',
      nationalIdentifier: '',
      source: '',
      sourceUrl: '',
      text: '',
      residenceMsForum: null,
      choiceCourt: null,
      choiceLaw: null
    };
  }

  ngOnInit() {

  }

  getSuggestionsFromApi(code: string) {
    this.http.get(`case/GetSuggestions/${code}`).subscribe((x: SuggestionsModel) => {
      this.courtSuggestions = x.courts || [];
      this.courtEngSuggestions = x.courtsEng || [];
      this.sourceSuggestions = x.sources || [];
      this.keywords = x.keywords || [];
    });
  }

  getEurovocData(data: TreeNode[]) {
    this.selectedTreeNodes = data;
    this.mod.eurovoc = [];
    for (const node of data) {
      this.mod.eurovoc.push({label: node.label, code: node.data});
    }
    this.euroVocChips = this.selectedTreeNodes.map(x => x.label);
    this.showTree = false;
  }

  removeChip(chip: { value: string }) {
    const value = chip.value;
    this.mod.eurovoc = this.mod.eurovoc.filter(x => x.label !== value);
    this.selectedTreeNodes = this.selectedTreeNodes.filter(x => x.label !== value);
  }

  addEulawLink() {
    this.editEuLinkIndex = -1;
    this.tmpEuLink = new IEulawLink();
    this.showEuLink = true;
  }

  showTreeFunc() {
    this.selectedTreeNodes = [...this.selectedTreeNodes];  // needed for when selection is cleared but tree is not saved
    this.showTree = true;
  }

  celexTester = /^(((0|3|6|2|4|5|7|8|e)(19|20)\d{2}[a-zA-Zа-яА-Я]{1,3}\d{4}([a-zA-Zа-яА-Я]{0,3}\(\d{2}\)([rR]\(\d{2}\))?)?(\-\d{8})?)|((119|120)\d{2})((?!r\(\d)[a-zA-Z])+((\/txt)?(r\(\d+\))?))|((019|020)\d{2}[a-zA-Z]{1}\/TXT\-\d{8})/i;

  validateEulawLink(link: IEubaseLink) {
    if (!this.celexTester.test(link.celex)) {
      if (!link.year) {
        this.alertService.error('Invalid field "Year"!');
        return false;
      }
      if ((link.year < 1900) || (link.year > 2030)) {
        this.alertService.error('Invalid field "Year"!');
        return false;
      }
    }
    if (!this.celexTester.test(link.celex)) {
      this.alertService.error('"invalid celex number: ' + link.celex);
      return false;
    }

    return true;
  }

  addEditEuLink() {
    if (+this.tmpEuLink.docType === 3) {
      if (!this.celexTester.test(this.tmpEuLink.celex)) {
        this.alertService.error('"invalid celex number: ' + this.tmpEuLink.celex);
        return;
      }
    }
    if (!this.validateEulawLink(this.tmpEuLink)) {
      return;
    }
    this.getNameFromEurLex(this.tmpEuLink);
    if (this.editEuLinkIndex > -1) {
      this.mod.euProvisions[this.editEuLinkIndex] = this.tmpEuLink;
    } else {
      this.mod.euProvisions.push(this.tmpEuLink);
    }
    this.showEuLink = false;
  }

  removeEuLink(itemToRemove: IEulawLink) {
    this.removeCitation(() => {
      this.mod.euProvisions = this.mod.euProvisions.filter(item => item !== itemToRemove);
    });
  }

  removeCitation(func: () => void) {
    this.confirmation.confirm({
      message: 'Do you want to delete this citation?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        func();
      }
    });
  }

  editEuLink(itemToEdit: IEulawLink, index: number) {
    this.editEuLinkIndex = index;
    this.tmpEuLink = JSON.parse(JSON.stringify(itemToEdit));
    this.showEuLink = true;
  }

  getNameFromEurLex(tmpLink: any) {
    this.http.get('/ReferenceEditor/actInfo?celex=' + tmpLink.celex).subscribe((data) => {
      const name: String = data['title'];
      if (name) {
        tmpLink.name = name; // (name.length > 100) ? name.substr(0, 100) + ' ...' : name;
      }
    });
  }

  addEditEuCase() {
    if (!this.validateEulawLink(this.tmpEucaseLink)) {
      return;
    }
    this.getNameFromEurLex(this.tmpEucaseLink);
    if (this.editEucaseIndex > -1) {
      this.mod.euCaselaw[this.editEucaseIndex] = this.tmpEucaseLink;
    } else {
      this.mod.euCaselaw.push(this.tmpEucaseLink);
    }
    this.showEuCaseLink = false;
  }

  addNationalLawItem(titleElement: HTMLInputElement, linkElement: HTMLInputElement) {
    const title = titleElement.value;
    const link = this.fixUrl(linkElement.value);
    if (!title) {
      this.alertService.error('Field title is required.');
      return;
    }
    if (this.editNatLawIndex !== undefined) {
      const item = this.mod.nationalLawRecords[this.editNatLawIndex];
      item.link = link;
      item.title = title;
    } else {
      this.mod.nationalLawRecords.push({title, link});
    }

    titleElement.value = '';
    linkElement.value = '';
    this.showAddNationalLaw = false;
  }

  addNationalCaseLawItem(titleElement: HTMLInputElement, identifierElement: HTMLInputElement, linkElement: HTMLInputElement) {
    const title = titleElement.value;
    const link = this.fixUrl(linkElement.value);
    const identifier = identifierElement.value;
    if (!title) {
      this.alertService.error('Field Title is required.');
      return;
    }
    if (!identifier) {
      this.alertService.error('Field Identifier is required.');
      return;
    }
    if (this.editNatCaseLawIndex !== undefined) {
      const item = this.mod.nationalCaseLawRecords[this.editNatCaseLawIndex];
      item.identifier = identifier;
      item.title = title;
      item.link = link;
    } else {
      this.mod.nationalCaseLawRecords.push({title, link, identifier});
    }
    titleElement.value = '';
    linkElement.value = '';
    identifierElement.value = '';
    this.showAddNationalCaseLaw = false;
  }

  removeEucaseLink(itemToRemove: IEucaseLink) {
    this.removeCitation(() => {
      this.mod.euCaselaw = this.mod.euCaselaw.filter(item => item !== itemToRemove);
    });
  }

  removeNationalLawRecord(itemToRemove: NationalLawRecord) {
    this.removeCitation(() => {
      this.mod.nationalLawRecords = this.mod.nationalLawRecords.filter(item => item !== itemToRemove);
    });
  }

  removeNationalCaseLawRecord(itemToRemove: NationalCaseLawRecord) {
    this.removeCitation(() => {
      this.mod.nationalCaseLawRecords = this.mod.nationalCaseLawRecords.filter(item => item !== itemToRemove);
    });
  }

  editEucaseLink(itemToEdit: IEucaseLink, index: number) {
    this.editEucaseIndex = index;
    this.tmpEucaseLink = JSON.parse(JSON.stringify(itemToEdit));
    this.showEuCaseLink = true;
  }

  editNatLawRecord(title: HTMLInputElement, link: HTMLInputElement, index: number) {
    const item = this.mod.nationalLawRecords[index];
    title.value = item.title;
    link.value = item.link || '';
    this.editNatLawIndex = index;
    this.showAddNationalLaw = true;
  }

  editNatCaseLawRecord(title: HTMLInputElement, link: HTMLInputElement, ident: HTMLInputElement, index: number) {
    const item = this.mod.nationalCaseLawRecords[index];
    title.value = item.title;
    link.value = item.link || '';
    ident.value = item.identifier || '';
    this.editNatCaseLawIndex = index;
    this.showAddNationalCaseLaw = true;
  }

  validateRequiredFields() {
    if (!this.mod.title || !this.mod.jurisdiction) {
      // if (!this.mod.title || !this.mod.keywords || !this.mod.summary || !this.mod.jurisdiction
      //   || !this.mod.court || !this.mod.dateOfDocument || !this.mod.language || !this.mod.decisionType
      // )

      this.alertService.error('Please fill "Title" field and select a Jurisdiction!');
      return false;
    }
    // if (this.mod.keywords.length === 0) {
    //   this.alertService.error('Please fill all mandatory fields!');
    //   return false;
    // }
    return true;
  }

  submitForm(needToClose: boolean) {
    this.submitted = true;
    if (!this.validateRequiredFields()) {
      return;
    }
    const content = JSON.stringify(this.mod);
    const date = new Date();
    const title = this.mod.title;
    const body = {title, content, date};
    const id = this.route.snapshot.params.id;
    if (id) {
      this.http.post(`case/EditCase/${id}`, body).subscribe(x => {
        this.alertService.success('Ok');
        if (needToClose) {
          this.router.navigate(['caselist']);
        }
      }, (error) => {
        this.alertService.error('Error');
      });
    } else {
      this.http.post('case/SaveCase', body).subscribe(idInserted => {
        this.alertService.success('Ok');
        if (needToClose) {
          this.router.navigate(['caselist']);
        } else {
          this.router.navigate([`caseeditor/${idInserted}`]);
        }
      }, (error) => {
        this.alertService.error('Error');
      });
    }
  }

  addEucaseLink() {
    this.editEucaseIndex = -1;
    this.tmpEucaseLink = new IEucaseLink();
    this.showEuCaseLink = true;
  }

  private mapEuroVocToTreeAndChips(eurovoc: INomenclature[]) {
    this.euroVocChips = eurovoc.map(x => x.label);
    this.selectedTreeNodes = eurovoc;
    // this.selectedTreeNodes = eurovoc.map(x => {label: x.label, });
  }

  addNationality() {
    this.showAddNationality = true;
  }

  addDomicile() {
    this.showAddDomicile = true;
  }

  addNationalityChip() {
    if (this.nationality) {
      if (!this.mod.nationalities) {
        this.mod.nationalities = [];
      }
      this.mod.nationalities.push(this.nationality);
      this.showAddNationality = false;
      this.nationality = '';
    } else {
      this.alertService.warn('Field nationality is required');
    }
  }

  addDomicileChip() {
    if (this.domicileCountry) {
      if (!this.mod.domiciles) {
        this.mod.domiciles = [];
      }
      const item = `${this.domicileCountry} (${this.domicileRole})`;
      const arr = [...this.mod.domiciles, item];
      this.mod.domiciles = arr.sort((a, b) => {
        const first = a.split(' ');
        const second = b.split(' ');
        return first[1].localeCompare(second[1]) || first[0].localeCompare(second[0]);
      });
      this.showAddDomicile = false;
      this.domicileCountry = '';
      this.domicileRole = "applicant";
    } else {
      this.alertService.warn('Field country is required');
    }
  }

  filterNationalities(text: { query: string }) {
    this.filteredNationalities = [];
    for (const nat of nationalities) {
      if (nat.toLowerCase().includes(text.query.toLowerCase())) {
        this.filteredNationalities.push(nat);
      }
    }
  }

  filterDomiciles(text: { query: string }) {
    this.filteredDomiciles = [];
    for (const nat of countries) {
      if (nat.toLowerCase().includes(text.query.toLowerCase())) {
        this.filteredDomiciles.push(nat);
      }
    }
  }

  filterKeywords(text: { query: string }) {
    this.filteredKeywords = [];
    for (const kw of this.keywords) {
      if (kw.toLowerCase().includes(text.query.toLowerCase())) {
        this.filteredKeywords.push(kw);
      }
    }
  }

  filterCourts(text: { query: string }) { // combine this with upper func
    this.courtSuggestionsFiltered = [];
    for (const court of this.courtSuggestions) {
      if (court.toLowerCase().includes(text.query.toLowerCase())) {
        this.courtSuggestionsFiltered.push(court);
      }
    }
  }

  filterCourtsEng(text: { query: string }) { // combine this with upper func
    this.courtEngSuggestionsFiltered = [];
    for (const court of this.courtEngSuggestions) {
      if (court.toLowerCase().includes(text.query.toLowerCase())) {
        this.courtEngSuggestionsFiltered.push(court);
      }
    }
  }

  filterSources(text: { query: string }) { // combine this with upper func
    this.sourceSuggestionsFiltered = [];
    for (const court of this.sourceSuggestions) {
      if (court.toLowerCase().includes(text.query.toLowerCase())) {
        this.sourceSuggestionsFiltered.push(court);
      }
    }
  }

  addKeyword() {
    if (this.keyword || this.keywordCheckboxes.length) {
      if (!this.mod.keywords) {
        this.mod.keywords = [];
      }
      if (this.keyword) {
        this.mod.keywords.push(this.keyword);
      }
      if (this.keywordCheckboxes.length) {
        const unique = new Set([...this.mod.keywords, ...this.keywordCheckboxes]);
        this.mod.keywords = Array.from(unique);
      }
      this.showKeyword = false;
      this.keywordCheckboxes = [];
    } else {
      this.alertService.warn('Keyword is required');
    }
  }

  onModelChange(event: INomenclature) {
    this.isEuDoc = false;
    this.isBgDoc = false;
    if (this.mod) {
      if (this.mod.jurisdiction) {
        if (this.mod.jurisdiction.code === 'EU') {
          this.isEuDoc = true;
        }
        if (this.mod.jurisdiction.code === 'BG') {
          this.isBgDoc = true;
        }
      }
    }
    if (event) {
      let codeForLanguage = event.code;
      const realCode = event.code;
      if (codeForLanguage === 'AT') { // fix for Austria and German
        codeForLanguage = 'DE';
      }
      const language = this.languages.find(x => x.code === codeForLanguage);
      this.mod.language = language;
      this.getSuggestionsFromApi(realCode);
    }
  }

  fillEudoc() {
    if (!this.validateEulawLink(this.tmpEuDoc)) {
      return;
    }
    this.fillModFromEurLex(this.tmpEuDoc);
    this.showEuDoc = false;
  }

  fillModFromEurLex(tmpLink: IEucaseLink) {
    this.http.get('/ReferenceEditor/actInfo?celex=' + tmpLink.celex).subscribe((data) => {
      if (data) {
        this.mod.title = data['title'];
        this.mod.ecli = data['ecli'];
        this.mod.dateOfDocument = new Date(data['date']);
        this.mod.court = tmpLink.court.label;
        this.mod.language = {label: 'English', code: 'EN'};
        this.mod.decisionType = tmpLink.docType;
        this.mod.source = 'EUR-Lex';
        this.mod.sourceUrl = 'https://eur-lex.europa.eu/legal-content/EN/ALL/?uri=CELEX:' + tmpLink.celex;
      }
    });
  }

  addKeywordsClick() {
    this.showKeyword = true;
    this.keyword = '';
  }

  previewUrl() {
    if (this.mod.sourceUrl) {
      const url = this.fixUrl(this.mod.sourceUrl);
      window.open(url);
    }
  }

  fixUrl(url: string): string {
    if (url) {
      const fixed = url.startsWith('http') ? url : 'http://' + url;
      return fixed;
    }
  }

  euLinkMouseEnter(euLinkItem: IEubaseLink) {
    this.linkHint = euLinkItem.name;
  }

  addNationalLaw(lawTitle: HTMLInputElement, lawLink: HTMLInputElement) {
    this.showAddNationalLaw = true;
    this.editNatLawIndex = undefined;
    lawTitle.value = '';
    lawLink.value = '';
  }

  addNationalCaseLaw(caseLawTitle: HTMLInputElement, caseLawLink: HTMLInputElement, caseLawIdent: HTMLInputElement) {
    this.showAddNationalCaseLaw = true;
    this.editNatCaseLawIndex = undefined;
    caseLawTitle.value = '';
    caseLawIdent.value = '';
    caseLawLink.value = '';
  }

  previewHtml(event, op: OverlayPanel) {
    if (!this.previewHtmlContent) {
      this.http.getText('./case/GetCaseHtmlContent/' + this.caseId).subscribe(x => {
        this.previewHtmlContent = x;
        op.show(event);
      });
    } else {
      op.show(event);
    }

  }
}


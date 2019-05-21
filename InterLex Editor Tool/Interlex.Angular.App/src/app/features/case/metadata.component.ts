import {Component, OnInit} from '@angular/core';
import {
  countryList,
  ICaseEditor, ICaseMetaContentResponse,
  IFile,
  IMetaEditor,
  INomenclature,
  languageList, yesNoRule
} from "../../models/case-editor.model";
import {ActivatedRoute, Router} from "@angular/router";
import {AlertService} from "../../core/services/alert.service";
import {ConfirmationService} from "primeng/api";
import {HttpService} from "../../core/services/http.service";
import {Constants} from "../../core/constants";
import {HttpResponse} from "@angular/common/http";
import {environment} from "../../../environments/environment";
import {OverlayPanel} from "primeng/primeng";

@Component({
  selector: 'app-metadata',
  templateUrl: './metadata.component.html',
  styleUrls: ['./metadata.component.css'],
  providers: [ConfirmationService]
})
export class MetadataComponent implements OnInit {

  model: IMetaEditor = {infoConsolidatedVersions: []};
  submitted = false;
  yesNoRule: INomenclature[];
  countries: INomenclature[];
  languages: INomenclature[];
  editable = true;
  environment = environment;
  actTypes: INomenclature[] = [{
    label: 'Instrument of primary legislation',
    code: 'I'
  }, {label: 'Instrument of secondary legislation', code: 'II'}];
  versions: INomenclature[] = [
    {label: 'Basic version', code: 'B'},
    {label: 'Latest consolidated version', code: 'L'}
  ];
  showAddConsolidated = false;
  editLinkIndex: number;
  private file: IFile;
  private translatedFile: IFile;
  metaId: string;
  files: { name: string }[];
  translatedFiles: { name: string }[];
  previewHtmlContent: string;

  constructor(private http: HttpService, private route: ActivatedRoute, private router: Router,
              private alertService: AlertService, private confirmation: ConfirmationService) {
    this.route.paramMap.subscribe((params) => {
      this.metaId = params.get('id');
      if (this.metaId) {
        this.initModelFromApi();
      }
    });
    this.yesNoRule = yesNoRule;
  }

  ngOnInit() {
    this.countries = countryList;
    this.languages = languageList;

  }

  submitForm(needToClose: boolean) {
    this.submitted = true;
    if (!this.validateRequiredFields()) {
      return;
    }
    const content = JSON.stringify(this.model);
    const title = this.model.title;
    const fileContent = this.file;
    const translatedFileContent = this.translatedFile;
    const body = {title, content, file: fileContent, translatedFile: translatedFileContent};
    const id = this.route.snapshot.params.id;
    if (id) {
      this.http.post(`case/EditMetadata/${id}`, body).subscribe(x => {
        this.alertService.success('Ok');
        if (needToClose) {
          this.router.navigate(['metalist']);
        }
      }, (error) => {
        this.alertService.error('Error');
      });
    } else {
      this.http.post('case/SaveMetadata', body).subscribe(idInserted => {
        this.alertService.success('Ok');
        if (needToClose) {
          this.router.navigate(['metalist']);
        } else {
          this.router.navigate([`metadata/${idInserted}`]);
        }
      }, (error) => {
        this.alertService.error('Error');
      });
    }
  }

  fixUrl(url: string): string {
    if (url) {
      const fixed = url.startsWith('http') ? url : 'http://' + url;
      return fixed;
    }
  }

  previewUrl(baseUrl: string) {
    if (baseUrl) {
      const url = this.fixUrl(baseUrl);
      window.open(url);
    }
  }

  validateRequiredFields() {
    if (!this.model.title || !this.model.jurisdiction) {
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

  addConsolidatedLinkItem(linkElement: HTMLInputElement) {
    const link = this.fixUrl(linkElement.value);
    if (this.editLinkIndex !== undefined) {
      this.model.infoConsolidatedVersions[this.editLinkIndex] = link;
    } else {
      this.model.infoConsolidatedVersions.push(link);
    }

    linkElement.value = '';
    this.showAddConsolidated = false;
  }

  addConsolidatedLink(link: HTMLInputElement) {
    this.showAddConsolidated = true;
    this.editLinkIndex = undefined;
    link.value = '';
  }

  editConsolidatedLink(link: HTMLInputElement, index: number) {
    const item = this.model.infoConsolidatedVersions[index];
    link.value = item;
    this.editLinkIndex = index;
    this.showAddConsolidated = true;
  }

  removeConsolidatedLink(index: number) {
    this.confirmation.confirm({
      message: 'Do you want to delete this citation?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.model.infoConsolidatedVersions.splice(index, 1);
      }
    });
  }

  onUpload(event: any, isTranslatedFile: boolean) { // this converts file to base64 string to put in JSON
    const file = event.files[0] as File;
    let fileReader = new FileReader();
    fileReader.readAsDataURL(file);
    fileReader.onload = () => {
      const result = fileReader.result as string;
      const base64 = result.split(',').pop();
      if (isTranslatedFile) {
        this.translatedFile = {base64Content: base64, filename: file.name, mimeType: file.type};
        this.model.translatedActFile = file.name;
      } else {
        this.file = {base64Content: base64, filename: file.name, mimeType: file.type};
        this.model.actFile = file.name;
      }
    };
  }

  removeFile(event: any) {
    this.file = null;
    this.model.actFile = null;
  }

  removeTranslatedFile(event: any) {
    this.translatedFile = null;
    this.model.translatedActFile = null;
  }

  private initModelFromApi() {
    const id = this.metaId;
    this.editable = false;
    this.http.get('./case/GetMetaContent/' + id).subscribe((response: ICaseMetaContentResponse) => {
      this.editable = response.editable;
      const data = JSON.parse(response.content) as IMetaEditor;
      this.model = {
        ...data,
        dateOfDocument: data.dateOfDocument == null ? null : new Date(data.dateOfDocument),
        dateEndValidity: data.dateEndValidity == null ? null : new Date(data.dateEndValidity),
        dateOfEffect: data.dateOfEffect == null ? null : new Date(data.dateOfEffect),
        dateOfPublication: data.dateOfPublication == null ? null : new Date(data.dateOfPublication),
        versionDateOfEffect: data.versionDateOfEffect == null ? null : new Date(data.versionDateOfEffect)
      };
      if (data.actFile) {  // fix for file list not populating from model, only from actual upload
        this.files = [{name: data.actFile}];
        this.file = {filename: data.actFile, mimeType: null, base64Content: null};
      }
      if (data.translatedActFile) {
        this.translatedFiles = [{name: data.translatedActFile}];
        this.translatedFile = {filename: data.translatedActFile, mimeType: null, base64Content: null};
      }
      // possible need to fix same problem as Case List with Austria/German
    }, (error) => {

    });
  }

  getFile(isTranslatedFile: boolean) {
    const url = isTranslatedFile ? 'case/GetMetaTranslatedFile/' : 'case/GetMetaFile/';
    this.http.getFile(url + this.metaId).subscribe((response: HttpResponse<Blob>) => {
      const fileName = response.headers.get('File-name');
      let dataType = response.body.type;
      let binaryData = [];
      binaryData.push(response.body);
      let downloadLink = document.createElement('a');
      downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, {type: dataType}));
      downloadLink.download = fileName;
      // if (filename)
      //   downloadLink.setAttribute('download', filename);
      document.body.appendChild(downloadLink);
      downloadLink.click();
      downloadLink.remove();
    });
  }

  previewHtml(event: MouseEvent, op: OverlayPanel) {
    if (!this.previewHtmlContent) {
      this.http.getText('./case/GetMetaHtmlContent/' + this.metaId).subscribe(x => {
        this.previewHtmlContent = x;
        op.show(event);
      });
    } else {
      op.show(event);
    }

  }
}

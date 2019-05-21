import {Component, Input, OnInit} from '@angular/core';
import {HttpService} from '../../core/services/http.service';
import {
  countryList,
  ICaseListRequestModel,
  ICaseListResponseModel,
  INomenclature
} from '../../models/case-editor.model';
import {Router} from '@angular/router';
import {AlertService} from '../../core/services/alert.service';
import {AuthService} from '../../core/services/auth.service';
import {OrganizationModel} from '../../models/organization.model';
import {ConfirmationService} from 'primeng/api';

@Component({
  selector: 'app-case-list',
  templateUrl: 'case-list.component.html',
  styleUrls: ['./case-list.component.css'],
  providers: [ConfirmationService]
})
export class CaseListComponent implements OnInit {
  @Input() type: 'case' | 'meta' = 'case';

  cols: any[];
  cases: ICaseListResponseModel[];
  requestModel: ICaseListRequestModel;
  organizations: OrganizationModel[];
  selectedOrg: OrganizationModel;
  loading = false;
  isAdmin: boolean;
  isSuperAdmin: boolean;
  countries: INomenclature[];
  jurisdiction: INomenclature;

  constructor(private http: HttpService, public router: Router, private alertService: AlertService, private auth: AuthService,
              private confirmation: ConfirmationService) {
    this.countries = countryList;
  }

  ngOnInit() {
    this.isAdmin = this.auth.isAdmin();
    this.isSuperAdmin = this.auth.isSuperAdmin();
    this.cols = [
      {field: 'userName', header: 'User'},
      {field: 'title', header: 'Case title'},
      {field: 'lastChange', header: 'Date'},
      {field: 'organization', header: 'Organization'},
    ];
    this.requestModel = {
      userName: '',
      pageNumber: 0,
      count: 0,
      organization: '',
      jurisdictionCode: ''
    };
    if (this.isSuperAdmin) {
      this.http.get('organization/GetOrganizationNames').subscribe((x: OrganizationModel[]) => {
        this.organizations = x;
      });
    }

    this.getCasesListFromApi();
  }

  getCasesListFromApi() {
    this.loading = true;
    this.makeApiCall();
  }

  filterCases() {
    if (this.selectedOrg && this.selectedOrg.shortName) {
      this.requestModel.organization = this.selectedOrg.shortName;
    } else {
      this.requestModel.organization = '';
    }
    // if (!this.requestModel.organization && !this.requestModel.userName) {
    //   return;
    // }
    this.getCasesListFromApi();
  }

  editCase(rowData: ICaseListResponseModel) {
    const path = this.type === "case" ? 'caseeditor' : 'metadata';
    this.router.navigate([path, rowData.caseId]);
  }

  deleteCase(rowData: ICaseListResponseModel) {
    this.confirmation.confirm({
      message: 'Do you want to delete this record?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        const path = this.type === "case" ? 'case/DeleteCase' : 'case/DeleteMeta';
        this.http.post(`${path}/${rowData.caseId}`, null)
          .subscribe(x => {
            this.alertService.success('Deleted successfully.');
            this.getCasesListFromApi();
          }, err => {
            this.alertService.error(err.error);
          });
      },
      reject: () => {
      }
    });
  }

  addNewCase() {
    const path = this.type === "case" ? 'caseeditor' : 'metadata';
    this.router.navigate([path]);
  }

  private makeApiCall() {
    const path = this.type === "case" ? 'case/GetCasesList':'case/GetMetaList';
    this.http.post(path, this.requestModel).subscribe((data: ICaseListResponseModel[]) => {
      this.cases = data;
      this.loading = false;
    }, (error) => {
      this.loading = false;
      this.alertService.error(error.message);
    });
  }

  onOrganizationChange(event) {
    this.filterCases();
  }

  onJurisdictionChange(event: INomenclature) {
    this.requestModel.jurisdictionCode = event ? event.code : '';
    this.getCasesListFromApi();
  }
}

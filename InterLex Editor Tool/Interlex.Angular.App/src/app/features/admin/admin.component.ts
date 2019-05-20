import {Component, OnInit} from '@angular/core';
import {HttpService} from '../../core/services/http.service';
import {ConfirmationService} from 'primeng/api';
import {AlertService} from '../../core/services/alert.service';
import {FormBuilder} from '@angular/forms';
import {AuthService} from '../../core/services/auth.service';
import {Router} from '@angular/router';
import {UserListModel} from '../../models/user-list.model';
import {OrganizationModel} from '../../models/organization.model';
import {UserService} from '../../core/services/user.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css'],
  providers: [ConfirmationService]
})
export class AdminComponent implements OnInit {

  users: UserListModel[] = [];
  isSuperAdmin = false;
  loading = false;
  organizations: OrganizationModel[];

  constructor(private http: HttpService, private confirmation: ConfirmationService, private alert: AlertService,
              private fb: FormBuilder, private auth: AuthService, private router: Router, private userService: UserService) {
  }

  ngOnInit() {
    this.getUsers();
    this.isSuperAdmin = this.auth.isSuperAdmin();
  }

  confirmDeactivate(username: string) {
    this.confirmation.confirm({
      message: 'Do you want to deactivate this user?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.http.post('users/DeactivateUser', {username})
          .subscribe(res => {
            this.alert.success('User deactivated');
            this.getUsers();
          });
      },
      reject: () => {
      }
    });
  }

  confirmActivate(username: string) {
    this.confirmation.confirm({
      message: 'Do you want to activate this user?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.http.post('users/ActivateUser', {username})
          .subscribe(res => {
            this.alert.success('User activated');
            this.getUsers();
          });
      },
      reject: () => {
      }
    });
  }

  editSuggestions() {
    this.router.navigate(['suggestions']);
  }

  getUsers() {
    this.loading = true;
    this.http.get('users/GetUsers').subscribe((users: UserListModel[]) => {
      this.users = users;
      this.loading = false;
      const orgs = new Set(users.map(u => u.organization));
      this.organizations = [];
      orgs.forEach(o => this.organizations.push({shortName: o} as OrganizationModel));
    }, error => {
      this.loading = false;
    });
  }

  registerUser() {
    this.router.navigate(['register']);
  }

  onOrganizationChange(org: { value: { shortName: string } }, table) {
    if (org.value) {
      table.filter(org.value.shortName, 'organization', 'equals');
    } else {
      table.reset();
    }
  }

  navigateChange(username: string) {
    this.userService.setUsernameToReset(username);
    this.router.navigate(['reset']);
  }
}



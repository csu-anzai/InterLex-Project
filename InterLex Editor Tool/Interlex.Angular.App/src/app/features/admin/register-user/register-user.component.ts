import {Component, OnInit} from '@angular/core';
import {OrganizationModel} from '../../../models/organization.model';
import {FormBuilder, FormGroup, ValidationErrors, Validators} from '@angular/forms';
import {HttpService} from '../../../core/services/http.service';
import {AlertService} from '../../../core/services/alert.service';
import {AuthService} from '../../../core/services/auth.service';
import {Router} from '@angular/router';
import {HttpErrorResponse} from '@angular/common/http';

@Component({
  selector: 'app-register-user',
  templateUrl: './register-user.component.html',
  styleUrls: ['./register-user.component.css']
})
export class RegisterUserComponent implements OnInit {

  organizations: OrganizationModel[];
  isSuperAdmin: boolean;
  loginForm: FormGroup;

  constructor(private http: HttpService, private alert: AlertService,
              private fb: FormBuilder, private auth: AuthService, private router: Router) {
  }

  ngOnInit() {
    this.isSuperAdmin = this.auth.isSuperAdmin();
    if (this.isSuperAdmin) {
      this.http.get('organization/GetOrganizationNames').subscribe((x: OrganizationModel[]) => {
        this.organizations = x;
      });
    }
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[\d])\S{6,}$/)]],
      confirmPassword: [''/*, [Validators.pattern(/^(?=.*[a-z])(?=.*[\d])\w{6,}$/)]*/],
      privileges: [0],
      organization: ['']
    }, {validator: this.passwordMatchValidator});
  }

  passwordMatchValidator(control: FormGroup): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
    if (password && confirmPassword && (password.value !== confirmPassword.value)) {
      confirmPassword.setErrors({'passwordMismatch': true});
    } else {
      confirmPassword.setErrors(null);
      return null;
    }
  }

  get f() {
    return this.loginForm.controls;
  }

  submit() {
    if (this.loginForm.invalid) {
      return;
    }
    this.http.post('users/RegisterUser', this.loginForm.value).subscribe(x => {
      this.alert.success('User registered!');
      this.router.navigate(['/admin']);
    }, (err: HttpErrorResponse) => {
      this.alert.error(JSON.stringify(err.error));
    });
  }
}

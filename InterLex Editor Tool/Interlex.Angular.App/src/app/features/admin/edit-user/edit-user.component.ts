import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ValidationErrors, Validators} from '@angular/forms';
import {HttpErrorResponse} from '@angular/common/http';
import {HttpService} from '../../../core/services/http.service';
import {AlertService} from '../../../core/services/alert.service';
import {AuthService} from '../../../core/services/auth.service';

@Component({
  selector: 'app-edit-user',
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.css']
})
export class EditUserComponent implements OnInit {

  editForm: FormGroup;

  constructor(private fb: FormBuilder, private http: HttpService, private alert: AlertService,
              private auth: AuthService) {
  }

  ngOnInit() {
    this.editForm = this.fb.group({
      oldPassword: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[\d])\S{6,}$/)]],
      confirmPassword: [''/*, [Validators.pattern(/^(?=.*[a-z])(?=.*[\d])\w{6,}$/)]*/],
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
    return this.editForm.controls;
  }

  submit() {
    if (this.editForm.invalid) {
      return;
    }
    this.http.post('users/EditUser', this.editForm.value).subscribe(x => {
      this.alert.success('Password changed successfully');
      this.loginUserNewToken();
      this.editForm.reset();
    }, (err: HttpErrorResponse) => {
      this.alert.error(JSON.stringify(err.error));
    });
  }

  private loginUserNewToken() {
    const username = this.auth.getUsername();
    const password = this.f.password.value;
    this.auth.login(username, password).subscribe();
  }
}

import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ValidationErrors, Validators} from '@angular/forms';
import {HttpService} from '../../../core/services/http.service';
import {AlertService} from '../../../core/services/alert.service';
import {HttpErrorResponse} from '@angular/common/http';
import {UserService} from '../../../core/services/user.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-reset-user',
  templateUrl: './reset-user.component.html',
  styleUrls: ['./reset-user.component.css']
})
export class ResetUserComponent implements OnInit, OnDestroy {

  editForm: FormGroup;
  username: string;

  constructor(private fb: FormBuilder, private http: HttpService, private alert: AlertService, private userService: UserService,
              private router: Router) {
  }

  ngOnInit() {
    const username = this.userService.getUsernameToReset();
    if (!username) {
      this.navigateToAdmin();
    }
    this.username = username;
    this.editForm = this.fb.group({
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
    this.http.post('users/ResetUser', {...this.editForm.value, username: this.username}).subscribe(x => {
      this.alert.success('Password changed successfully');
      this.navigateToAdmin();
    }, (err: HttpErrorResponse) => {
      this.alert.error(JSON.stringify(err.error));
    });
  }

  navigateToAdmin() {
    this.router.navigate(['admin']);
  }

  ngOnDestroy(): void {
    this.userService.setUsernameToReset(undefined);
  }
}

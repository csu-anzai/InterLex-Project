import {Injectable} from '@angular/core';
import {HttpService} from './http.service';
import {map} from 'rxjs/operators';
import {User, UserPrivileges} from '../../models/user.model';
import {BehaviorSubject, Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  loggedIn = false; // alternative to checking localstorage?
  private subject = new BehaviorSubject<boolean>(this.isLoggedIn());

  constructor(private http: HttpService) {
  }

  login(username: string, password: string) {
    return this.http.post('auth/login', {username, password})
      .pipe(map(user => {
        if (user && user.token) {
          localStorage.setItem('currentUser', JSON.stringify(user));
          this.subject.next(true);
        }
        return user;
      }));
  }

  logout() {
    localStorage.removeItem('currentUser');
    this.subject.next(false);
  }

  isLoggedIn() {
    return localStorage.getItem('currentUser') !== null;
  }

  isAdmin() {
    const user = localStorage.getItem('currentUser');
    if (user) {
      const parsedUser: User = JSON.parse(user);
      if (parsedUser.privileges === UserPrivileges.Admin || parsedUser.privileges === UserPrivileges.SuperAdmin) {
        return true;
      }
    }
    return false;
  }

  isSuperAdmin() {
    const user = localStorage.getItem('currentUser');
    if (user) {
      const parsedUser: User = JSON.parse(user);
      if (parsedUser.privileges === UserPrivileges.SuperAdmin) {
        return true;
      }
    }
    return false;
  }

  getUsername() {
    const user = localStorage.getItem('currentUser');
    if (user) {
      const parsed: User = JSON.parse(user);
      return parsed.username;
    }
  }

  getToken() {
    const user = localStorage.getItem('currentUser');
    if (user) {
      const parsed: User = JSON.parse(user);
      return parsed.token;
    }
  }

  getLoggedInStatus(): Observable<boolean> {
    return this.subject.asObservable();
  }
}

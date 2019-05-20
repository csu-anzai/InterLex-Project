import {Injectable} from '@angular/core';
import {Constants} from '../constants';
import {User} from '../../models/user.model';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private username: string;

  constructor(private httpClient: HttpClient) { // check how to do this with api service - overloads for all methods??
  }

  getAll() {
    const result = this.httpClient.get<User[]>(Constants.API_BASE + 'users');
    return result;
  }

  setUsernameToReset(username: string) {
    this.username = username;
  }

  getUsernameToReset() {
    return this.username;
  }
}

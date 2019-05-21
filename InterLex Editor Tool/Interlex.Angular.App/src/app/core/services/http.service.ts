import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Constants} from '../constants';

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  constructor(private http: HttpClient) {
  }

  get(url: string): Observable<object> {
    return this.http.get(Constants.API_BASE + url);
  }

  post(url: string, model: any): Observable<any> {
    const body = JSON.stringify(model);
    const headers = new HttpHeaders({'Content-Type': 'application/json'}); // add auth here vs interceptor?
    const options = {headers};
    return this.http.post(Constants.API_BASE + url, body, options);
  }

  getFile(url: string) {
    return this.http.get(Constants.API_BASE + url, {responseType: 'blob' as 'json', observe: "response"});
  }

  getText(url: string): Observable<string> {
    return this.http.get(Constants.API_BASE + url, {responseType: "text"});
  }
}

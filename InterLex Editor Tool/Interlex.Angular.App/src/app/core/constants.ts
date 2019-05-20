import { environment } from '../../environments/environment';
export class Constants {
  // public static API_BASE = 'https://localhost:44388/api/';
  // public static get API_BASE(): string { return environment.apiBaseUrl; } // 'http://192.168.2.16:8008/webapi/api/';
  public static API_BASE = environment.apiBaseUrl;  // 'http://192.168.2.16:8008/webapi/api/';
}

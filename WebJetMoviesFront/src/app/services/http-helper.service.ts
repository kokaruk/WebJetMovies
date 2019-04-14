import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpHelperService {
  private readonly baseURL = 'https://staging.kokaruk.com/webjet/api';

  constructor(private http: HttpClient) {
  }

  get<T>(url: string): Observable<T> {
    return this.http.get<T>(url);
  }

  buildURL(endpoint: string, id?: string): string {
    const apiEndpoint = `${this.baseURL}/${endpoint}`;
    return id ? `${apiEndpoint}/${id}` : apiEndpoint;
  }

  private furnishHeaders(): any {
    // TODO for tokenization service
  }
}

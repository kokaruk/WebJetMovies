import { Injectable } from '@angular/core';
import { HttpHelperService } from './http-helper.service';
import { IUser } from '../models/IUser';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly endpoint = 'users';

  constructor(private http: HttpHelperService) { }

  getAllUsers(): Observable<Array<IUser>> {
    return this.http.get(this.http.buildURL(this.endpoint));
  }
}

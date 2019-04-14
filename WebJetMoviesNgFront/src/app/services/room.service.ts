import {Injectable} from '@angular/core';
import {HttpHelperService} from './http-helper.service';
import {IRoom} from '../models/IRoom';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private readonly endpoint = 'rooms';

  constructor(private http: HttpHelperService) {
  }

  getRooms(): Observable<Array<IRoom>> {
    return this.http.get<Array<IRoom>>(this.http.buildURL(this.endpoint));
  }

  addRoom(data: Partial<IRoom>): Observable<IRoom> {
    return this.http.post<IRoom>(this.http.buildURL(this.endpoint), data);
  }

  editRoom(data: Partial<IRoom>): Observable<IRoom> {
    return this.http.put<IRoom>(this.http.buildURL(this.endpoint), data);
  }

  deleteRoom(data: Partial<IRoom>): Observable<IRoom> {
    return this.http.delete(this.http.buildURL(this.endpoint, data.roomID));
  }
}

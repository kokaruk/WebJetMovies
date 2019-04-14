import { Injectable } from '@angular/core';
import { ISlot } from 'src/app/models/ISlot';
import { Observable } from 'rxjs';
import { HttpHelperService } from './http-helper.service';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private readonly endpoint = 'slots';
  constructor(private http: HttpHelperService) { }

  getAllSlots(): Observable<Array<ISlot>> {
    return this.http.get<Array<ISlot>>(this.http.buildURL(this.endpoint));
  }

  editSlot(slot: ISlot): Observable<ISlot> {
    return this.http.put<ISlot>(this.http.buildURL(this.endpoint), slot);
  }

  deleteSlot(slot: ISlot): Observable<ISlot> {
    return this.http.delete(this.http.buildURL(this.endpoint, `${slot.roomID}/${slot.startTime}`));
  }
}

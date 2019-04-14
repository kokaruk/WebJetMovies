import {Injectable} from '@angular/core';
import {ICollectionResponse} from '../models/ICollectionResponse';
import {Observable} from 'rxjs';
import {HttpHelperService} from './http-helper.service';
import {IMovieResponse} from '../models/IMovieResponse';

@Injectable({
  providedIn: 'root'
})
export class MoviesService {

  constructor(private http: HttpHelperService) {
  }

  getAllMovies(page: string): Observable<ICollectionResponse> {
    return this.http.get<ICollectionResponse>(this.http.buildURL(page));
  }

  getMovie(pageUrl: string): Observable<IMovieResponse> {
    return this.http.get<IMovieResponse>(this.http.buildURL(pageUrl));
  }

}

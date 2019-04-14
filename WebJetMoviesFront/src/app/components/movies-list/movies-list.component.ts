import {Component, OnInit} from '@angular/core';
import {MoviesService} from '../../services/movies.service';
import {ICollectionResponse} from '../../models/ICollectionResponse';

@Component({
  selector: 'app-movies-list',
  templateUrl: './movies-list.component.html',
  styleUrls: ['./movies-list.component.css']
})
export class MoviesListComponent implements OnInit {

  productsObservable: ICollectionResponse;
  pageUrl: string;

  constructor(private moviesService: MoviesService) {
    this.pageUrl = '/movies/1';
  }


  ngOnInit() {
    this.getAllMovies();
  }

  private getAllMovies(): void {
    this.moviesService.getAllMovies(this.pageUrl).subscribe(
      res => {
        this.productsObservable = res;
      });
  }

  setPage(pageUrl: string) {
    this.pageUrl = pageUrl;
    this.getAllMovies();
  }

}

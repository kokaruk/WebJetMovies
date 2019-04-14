import {Component, OnInit} from '@angular/core';
import {MovieService} from '../../services/movie.service';
import {IMovie} from '../../models/IMovie';

@Component({
  selector: 'app-movies-all',
  templateUrl: './movies-all.component.html',
  styleUrls: ['./movies-all.component.css']
})
export class MoviesAllComponent implements OnInit {

  movies: Array<IMovie> = [];

  constructor(
    private moviesService: MovieService
  ) {
  }

  ngOnInit() {
    // this.getAllMovies();
  }

  private getAllMovies(): void {
    this.moviesService.getAllMovies().subscribe(
      movies => {
        this.movies = movies;
      }
    );
  }

}

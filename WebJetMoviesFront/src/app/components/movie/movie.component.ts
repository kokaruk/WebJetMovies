import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {IMovieResponse} from '../../models/IMovieResponse';
import {MoviesService} from '../../services/movies.service';

@Component({
  selector: 'app-movie',
  templateUrl: './movie.component.html',
  styleUrls: ['./movie.component.css']
})
export class MovieComponent implements OnInit {

  movieResponse: IMovieResponse;

  constructor(private activatedRoute: ActivatedRoute, private moviesService: MoviesService) {
    this.getMovie(activatedRoute.snapshot.params.year,
      activatedRoute.snapshot.params.title);
  }

  ngOnInit() {
  }

  private getMovie(year: string, title: string): void {
    const param = `movies/${year}/${title}`;
    this.moviesService.getMovie(param).subscribe(
      res => {
        this.movieResponse = res;
      });
  }


}

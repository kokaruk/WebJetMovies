import {IMovie} from './IMovie';

export interface ICollectionResponse {
  items: Array<IMovie>;
  nextPage: string;
  previousPage: string;
}

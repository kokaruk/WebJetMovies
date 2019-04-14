import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {PageNotFoundComponent} from './components/page-not-found/page-not-found.component';
import {BookingComponent} from './components/dashboard/booking/booking.component';
import {RoomComponent} from './components/dashboard/room/room.component';
import {HomeComponent} from './components/home/home.component';
import {DashboardComponent} from './components/dashboard/dashboard.component';
import {MoviesAllComponent} from './components/movies-all/movies-all.component';

const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path: 'home', component: MoviesAllComponent},
  {
    path: 'dashboard', component: DashboardComponent,
    children: [
      {
        path: '',
        redirectTo: 'booking',
        pathMatch: 'full'
      },
      {
        path: 'booking',
        component: BookingComponent
      },
      {
        path: 'room',
        component: RoomComponent
      }
    ]
  },
  {path: '**', component: PageNotFoundComponent},
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {
}




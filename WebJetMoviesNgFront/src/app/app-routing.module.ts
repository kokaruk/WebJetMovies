import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import {BookingComponent} from './components/dashboard/booking/booking.component';
import {RoomComponent} from './components/dashboard/room/room.component';
import {HomeComponent} from './components/home/home.component';
import {DashboardComponent} from './components/dashboard/dashboard.component';


const routes: Routes = [
  { path: '',   redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'dashboard', component: DashboardComponent,
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
    ]},
  { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }




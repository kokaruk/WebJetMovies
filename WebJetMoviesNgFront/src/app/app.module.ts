import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TimeslotComponent } from './components/common/timeslot/timeslot.component';
import { TimetableComponent } from './components/common/timetable/timetable.component';
import { DashboardModule } from './components/dashboard/dashboard.module';
import { MaterialModule } from './modules/material.module';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { HttpClientModule } from '@angular/common/http';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { NavToolbarComponent } from './components/nav-toolbar/nav-toolbar.component';
import { HomeComponent } from './components/home/home.component';
import { FooterComponent } from './footer/footer.component';
import { MoviesAllComponent } from './components/movies-all/movies-all.component';

@NgModule({
  declarations: [
    AppComponent,
    TimeslotComponent,
    TimetableComponent,
    PageNotFoundComponent,
    NavToolbarComponent,
    HomeComponent,
    FooterComponent,
    MoviesAllComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    DashboardModule,
    MaterialModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

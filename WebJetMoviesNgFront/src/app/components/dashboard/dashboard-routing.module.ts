import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DashboardComponent } from './dashboard.component';
import { RoomComponent } from './room/room.component';
import { BookingComponent } from './booking/booking.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../modules/material.module';

const dashboardRoutes: Routes = [];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild(dashboardRoutes),
    MaterialModule
  ],
  declarations: [
    DashboardComponent,
    RoomComponent,
    BookingComponent
  ],
  exports: [
    RouterModule
  ]
})
export class DashboardRoutingModule { }


/*
Copyright 2017-2018 Google Inc. All Rights Reserved.
Use of this source code is governed by an MIT-style license that
can be found in the LICENSE file at http://angular.io/license
*/

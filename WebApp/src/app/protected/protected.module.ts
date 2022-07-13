import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProtectedRoutingModule } from './protected-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { RegistrationsComponent } from './registrations/registrations.component';

@NgModule({
  declarations: [
    DashboardComponent,
    RegistrationsComponent
  ],
  imports: [
    CommonModule,
    ProtectedRoutingModule,
    MatButtonModule,
    MatTableModule,
  ]
})
export class ProtectedModule { }

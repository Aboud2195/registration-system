import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';

import { UserDto } from './public/interfaces';

// specify the key where the token is stored in the local storage
export const LOCALSTORAGE_USER_KEY = 'user';

export function userGetter(): UserDto | null {
  const stored = localStorage.getItem(LOCALSTORAGE_USER_KEY);
  if (!stored) {
    return null;
  }

  const user = JSON.parse(stored);
  if (!user) {
    return null;
  }
  
  console.log(user);
  return user;
}

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    // Import our Routes for this module
    AppRoutingModule,
    // Angular Material Imports
    MatSnackBarModule,
    MatTableModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

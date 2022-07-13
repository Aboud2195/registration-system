import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { tap } from 'rxjs';
import { LOCALSTORAGE_USER_KEY, userGetter } from 'src/app/app.module';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {

  constructor(
    private router: Router,
    private http: HttpClient
  ) { }

  isAdmin = userGetter()?.role == 'Admin';

  logout() {
    // Removes the jwt token from the local storage, so the user gets logged out & then navigate back to the "public" routes
    
    this.http.get('/api/Account/Logout').pipe(tap(() => {
      localStorage.removeItem(LOCALSTORAGE_USER_KEY);
      this.router.navigate(['../../']);
    })).subscribe();

  }

}

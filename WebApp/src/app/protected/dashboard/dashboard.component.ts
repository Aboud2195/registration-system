import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { tap } from 'rxjs';
import { LOCALSTORAGE_USER_KEY, userGetter } from 'src/app/app.module';
import { HttpClient } from '@angular/common/http';
import { UserDto } from 'src/app/public/interfaces';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {

  user: UserDto;
  isAdmin: boolean;

  constructor(
    private router: Router,
    private http: HttpClient
  ) {
    this.user = userGetter()!;
    this.isAdmin = this.user.role == 'Admin';
  }



  logout() {
    this.http.get('/api/Account/Logout').pipe(tap(() => {
      localStorage.removeItem(LOCALSTORAGE_USER_KEY);
      this.router.navigate(['../../']);
    })).subscribe();

  }

}

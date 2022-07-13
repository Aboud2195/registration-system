import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ChangeDetectionStrategy, ViewChild } from '@angular/core';
import { tap } from 'rxjs';
import { UserDto } from 'src/app/public/interfaces';
import { MatTable } from '@angular/material/table';

@Component({
  selector: 'app-registrations',
  templateUrl: './registrations.component.html',
  styleUrls: ['./registrations.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class RegistrationsComponent implements OnInit {

  @ViewChild(MatTable) table!: MatTable<UserDto>;
  constructor(
    private http: HttpClient) { }

  dataSource: UserDto[] = [];
  displayedColumns: string[] = ['email', 'firstName', 'lastName', 'approve', 'reject'];
  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.http.get<UserDto[]>('/api/Account/Pending').pipe(tap((result: UserDto[]) => {
      this.dataSource.splice(0, this.dataSource.length);
      result.forEach(user => {
        this.dataSource.push(user);
      });
      this.table.renderRows();
    })).subscribe();
  }

  approveUser(id: string) {
    this.http.put(`/api/Account/${id}/Approve`, {}).pipe(tap(() => {
      this.loadUsers();
    })).subscribe();
  }

  rejectUser(id: string) {
    this.http.delete(`/api/Account/${id}`, {}).pipe(tap(() => {
      this.loadUsers();
    })).subscribe();
  }
}
import { Component } from '@angular/core';
import { USERS } from './mock-user';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent {
  dataSource = USERS;
  displayedColumns: string[] = ['username', 'mail', 'firstname', 'lastname'];

  constructor() { }


}

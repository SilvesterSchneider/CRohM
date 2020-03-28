import { Component } from '@angular/core';
import { USERS } from './mock-user';
import { FormBuilder, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { UserDto } from '../../shared/api-generated/api-generated';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent {
  dataSource = new BehaviorSubject<UserDto[]>(USERS);
  displayedColumns: string[] = ['username', 'mail', 'firstname', 'lastname'];

  userForm = this.fb.group({
    mail: ['', [Validators.email, Validators.required]],
    vorname: ['', Validators.required],
    nachname: ['', Validators.required]
  });

  constructor(private fb: FormBuilder) { }

  public addUser() {
    // TODO: Replace Mock logic with backend call
    USERS.push(this.userForm.value);
    this.dataSource.next(USERS);
  }

}

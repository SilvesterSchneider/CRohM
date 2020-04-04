import { Component } from '@angular/core';
import { USERS } from './mock-user';
import { FormBuilder, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { UserDto, UsersService, AuthService } from '../../shared/api-generated/api-generated';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent {
  dataSource = new BehaviorSubject<UserDto[]>(USERS);
  displayedColumns: string[] = ['username', 'mail', 'firstname', 'lastname', 'option'];

  userForm = this.fb.group({
    email: ['', [Validators.email, Validators.required]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required]
  });

  constructor(
    private fb: FormBuilder,
    private usersService: UsersService,
    private readonly authService: AuthService) { }


    public OnDelete(userId: number) {
      // TODO: call backend delete function
    }


  public OnPasswordReset(userId: number) {
    this.authService.changePassword(userId).subscribe(result => {
      console.log(result);
    });
  }

  public addUser() {
    // TODO: Update list after successful call, not yet implemented in backend
    this.usersService.post(this.userForm.value).subscribe();
  }

}

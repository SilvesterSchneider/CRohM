import { Component, OnInit } from '@angular/core';
import { USERS } from './mock-user';
import { FormBuilder, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { UserDto, UsersService, AuthService } from '../../shared/api-generated/api-generated';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})

export class UserComponent implements OnInit {
  dataSource = new BehaviorSubject<UserDto[]>([]);
  displayedColumns: string[] = ['username', 'mail', 'firstname', 'lastname', 'option'];


  userForm = this.fb.group({
    email: ['', [Validators.email, Validators.required]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required]
  });


  constructor(private readonly fb: FormBuilder,
              private readonly usersService: UsersService,
              private readonly authService: AuthService) { }
  public ngOnInit(): void {
   this.GetData();
  }


private GetData() {
  this.usersService.get().subscribe(users => {
    this.dataSource.next(users);
});
}



    public OnDelete(userId: number) {
      // TODO: call backend delete function
    }


  public OnPasswordReset(userId: number) {
    this.authService.changePassword(userId).subscribe(result => {
      console.log(result);
    });
  }

  public addUser() {
    this.usersService.post(this.userForm.value).subscribe(user => {
      console.log(user);
      this.GetData();
    });
  }

}

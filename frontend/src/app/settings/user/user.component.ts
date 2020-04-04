import { Component, OnInit } from '@angular/core';
import { USERS } from './mock-user';
import { FormBuilder, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { UserDto, UsersService } from '../../shared/api-generated/api-generated';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  dataSource = new BehaviorSubject<UserDto[]>([]);
  displayedColumns: string[] = ['username', 'mail', 'firstname', 'lastname'];

  userForm = this.fb.group({
    email: ['', [Validators.email, Validators.required]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required]
  });

  constructor(private readonly fb: FormBuilder,
              private readonly usersService: UsersService) { }
  public ngOnInit(): void {
   this.GetData();
  }


private GetData() {
  this.usersService.get().subscribe(users => {
    this.dataSource.next(users);
});
}



  public addUser() {
    this.usersService.post(this.userForm.value).subscribe(user => {
      console.log(user);
      this.GetData();
    });
  }

}

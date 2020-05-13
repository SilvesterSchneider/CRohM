import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserDto, UsersService, AuthService } from '../../shared/api-generated/api-generated';
import { MatTable } from '@angular/material/table';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})

export class UserComponent implements OnInit {
  @ViewChild(MatTable) table: MatTable<any>;
  dataSource = new BehaviorSubject<UserDto[]>([]);
  displayedColumns: string[] = ['username', 'mail', 'firstname', 'lastname', 'options'];

  userForm = this.fb.group({
    email: ['', [Validators.email, Validators.required]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required]
  });


  constructor(
    private readonly fb: FormBuilder,
    private readonly usersService: UsersService,
    private readonly authService: AuthService) { }
  public ngOnInit(): void {
    this.GetData();
  }


  private GetData() {
    this.usersService.get().subscribe(x => {
      this.dataSource.next(x);
      this.table.renderRows();
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

  public SetLockoutState(userId: number) {
    this.usersService.updateLockoutState(userId).subscribe(x => this.GetData());
  }
}

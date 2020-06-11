import { Component, OnInit, ViewChild } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { UserDto, UsersService, AuthService } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table';
import { AddUserDialogComponent } from './add-user/add-user.component';
import { EditUserDialogComponent } from './edit-user/edit-user.component';


@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})

export class UserComponent implements OnInit {
  @ViewChild(MatTable) table: MatTable<any>;
  dataSource = new BehaviorSubject<UserDto[]>([]);
  displayedColumns: string[] = ['username', 'mail', 'firstname', 'lastname', 'options'];

  constructor(// private readonly fb: FormBuilder,
    private readonly usersService: UsersService,
    private readonly authService: AuthService,
    public dialog: MatDialog) { }

  public ngOnInit(): void {
    this.GetData();
  }

  /**
   * Oeffnen des Dialogs zum Hinzufuegen eines neuen Nutzers
   */
  openAddDialog(): void {
    const dialogRef = this.dialog.open(AddUserDialogComponent, {
      width: '400px',
    });

    dialogRef.afterClosed().subscribe(result => {
      // Update der angezeigten User-Liste
      this.GetData();
    });
  }

  /**
   * Oeffnen des Dialogs zum Bearbeiten eines Nutzers
   */
  openEditDialog(): void {
    const dialogRef = this.dialog.open(EditUserDialogComponent, {
      width: '400px',
    });

    dialogRef.afterClosed().subscribe(result => {
      // Update der angezeigten User-Liste
      this.GetData();
    });
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

  public SetLockoutState(userId: number) {
    this.usersService.updateLockoutState(userId).subscribe(x => this.GetData());
  }
}



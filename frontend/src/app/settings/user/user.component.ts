import { Component, OnInit, ViewChild } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { UserDto, UsersService, AuthService } from '../../shared/api-generated/api-generated';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table';
import { AddUserDialogComponent } from './add-user/add-user.component';
import { EditUserDialogComponent } from './edit-user/edit-user.component';
import { DeleteEntryDialogComponent } from '../../shared/form/delete-entry-dialog/delete-entry-dialog.component';
import { JwtService } from 'src/app/shared/jwt.service';



@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})

export class UserComponent implements OnInit {
  @ViewChild(MatTable) table: MatTable<any>;
  dataSource = new BehaviorSubject<UserDto[]>([]);
  displayedColumns: string[] = ['username', 'mail', 'firstname', 'lastname', 'options'];
  permissionAddUser = false;
  permissionLockUser = false;
  permissionResetPasswort = false;
  permissionEditUser = false;


  constructor(// private readonly fb: FormBuilder,
    private readonly usersService: UsersService,
    private readonly authService: AuthService,
    public dialog: MatDialog,
    private jwt: JwtService) { }

  public ngOnInit(): void {
    this.GetData();
    this.permissionAddUser = this.jwt.hasPermission('Anlegen eines Benutzers');
    this.permissionLockUser = this.jwt.hasPermission('Löschen / Deaktivieren eines Benutzers');
    this.permissionResetPasswort = this.jwt.hasPermission('Rücksetzen eines Passworts eines Benutzers');
    this.permissionEditUser = this.jwt.hasPermission('Zuweisung einer neuen Rolle zu einem Benutzer');
  }

  /**
   * Oeffnen des Dialogs zum Hinzufuegen eines neuen Nutzers
   */
  openAddDialog(): void {
    const dialogRef = this.dialog.open(AddUserDialogComponent, {
      width: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      // Update der angezeigten User-Liste
      this.GetData();
    });
  }

  /**
   * Oeffnen des Dialogs zum Bearbeiten eines Nutzers
   */
  openEditDialog(userId: number): void {
    this.usersService.get().subscribe(x => {
      this.finalizeOpenEditDialog(x.find(a => a.id === userId));
    });
  }

  finalizeOpenEditDialog(user: UserDto) {
    const dialogRef = this.dialog.open(EditUserDialogComponent, {
      data: user,
      width: '400px',
      disableClose: true
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
    const deleteDialogRef = this.dialog.open(DeleteEntryDialogComponent, {
      data: 'Benutzer',
      disableClose: true
    });

    deleteDialogRef.afterClosed().subscribe((deleteResult) => {
      if (deleteResult.delete) {
        // TODO: call backend delete function
      }
    });

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

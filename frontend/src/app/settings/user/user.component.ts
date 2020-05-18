import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, Validators, FormControl } from '@angular/forms';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserDto, UsersService, AuthService } from '../../shared/api-generated/api-generated';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConfirmDialogModel, ConfirmDialogComponent } from '../../confirmdialog/confirmdialog.component';
import { MatTable } from '@angular/material/table';



interface Permission {
  value: string;
  viewValue: string;
}



// export interface DialogData {
//   username: string;
//   mail: string;
//   firstname: string;
//   lastname: string;
// }


@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})

export class UserComponent implements OnInit {
  @ViewChild(MatTable) table: MatTable<any>;
  dataSource = new BehaviorSubject<UserDto[]>([]);
  displayedColumns: string[] = ['username', 'mail', 'firstname', 'lastname', 'options'];

  // userForm = this.fb.group({
  //   email: ['', [Validators.email, Validators.required]],
  //   firstName: ['', Validators.required],
  //   lastName: ['', Validators.required]
  // });


  constructor(// private readonly fb: FormBuilder,
    private readonly usersService: UsersService,
    private readonly authService: AuthService,
    public addDialog: MatDialog) { }
    
  public ngOnInit(): void {
    this.GetData();
  }

  /**
   * Oeffnen des Dialogs zum Hinzufuegen eines neuen Nutzers
   */
  openDialog(): void {
    const dialogRef = this.addDialog.open(UserDialogAddComponent, {
      width: '500px',
    });

    dialogRef.afterClosed().subscribe(result => {
      // Ausgabe auf der Konsole zum Debuggen
      // console.log('The dialog was closed');
      // Update der angezeigten User-Liste
      this.GetData();
    });
  }

  // private GetData() {
  //   this.usersService.get().subscribe(users => {
  //     this.dataSource.next(users);
  //   });
  // }



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

  // public addUser() {
  //   this.usersService.post(this.userForm.value).subscribe(user => {
  //     console.log(user);
  //     this.GetData();
  //   });
  // }

  public SetLockoutState(userId: number) {
    this.usersService.updateLockoutState(userId).subscribe(x => this.GetData());
  }

}








/**
 * Komponente fuer den Modal-Dialog zum Hinzufuegen eines Nutzers
 */
@Component({
  // selector: 'user.component_dialog_add',
  templateUrl: 'user.component_dialog_add.html',
})
export class UserDialogAddComponent {

  userForm = this.fb.group({
    email: ['', [Validators.email, Validators.required]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required]
  });

  // Gruppenberechtigungen
  // TODO: Liste aus Backend laden
  groupPermissionList: Permission[] = [
    { value: 'group_adm', viewValue: 'Administrator' },
    { value: 'group_dat', viewValue: 'Datenschutzbeauftragter' },
    { value: 'group_hig', viewValue: 'Hoch' },
    { value: 'group_med', viewValue: 'Normal' },
    { value: 'group_low', viewValue: 'Eingeschränkt' }
  ];
  groupPermissionDefault = ['group_low'];
  groupPermission = new FormControl(this.groupPermissionDefault);


  constructor(
    private readonly fb: FormBuilder,
    private readonly usersService: UsersService,
    public dialogRef: MatDialogRef<UserDialogAddComponent>,
    public dialog: MatDialog,
    // @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) { }


  /**
   * Funktion zum Schliessen des Dialogs
   */
  abortDialog(): void {
    // Schliessen des Dialogs
    this.dialogRef.close();
  }


  /**
   * Funktion zum Speichen des neuen Users und Schliessen des Dialogs
   */
  public addUser() {
    this.usersService.post(this.userForm.value).subscribe(user => {
      // Ausgabe der Daten auf der Konsole zum Debuggen
      // console.log(user);
    });

    // TODO: Speichern/Uebermitteln der Gruppenbrerchtigungen
    // Ausgabe der Daten auf der Konsole zum Debuggen
    console.log(this.groupPermission.value);

    // Schliessen des Dialogs
    this.dialogRef.close();
  }


  /**
   * Funktion zum Aufruf des Confirm-Dialogs
   */
  confirmDialog(): void {
    // Angezeigte Ueberschrift bzw. Nachricht im Confirm-Dialog
    const message = `Wollen Sie den Vorgang wirklich abbrechen?`;
    const dialogData = new ConfirmDialogModel('Warnung', message);

    // Oeffnet den Confirm-Dialog mit definierter Ueberschrift bzw. Nachricht
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    // Schliesst den Confirm-Dialog
    dialogRef.afterClosed().subscribe(dialogResult => {
      // Wenn Nein angeklickt wurde passiert nichts.
      // Wenn 'Ja' angeklickt wurde, dann wird die abortDialog-Funktion
      // des drunterliegenden Dialogs (hier also Dialog zum Hinzufuegen
      // eines Nutzers) geschlossen.
      if (dialogResult === true) {
        this.abortDialog();
      }
    });
  }

  
}

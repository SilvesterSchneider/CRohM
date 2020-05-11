import { Component, OnInit, Inject } from '@angular/core';
import { USERS } from './mock-user';
import { FormBuilder, Validators, FormControl } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { UserDto, UsersService, AuthService } from '../../shared/api-generated/api-generated';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';



interface permission {
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
  dataSource = new BehaviorSubject<UserDto[]>([]);
  displayedColumns: string[] = ['username', 'mail', 'firstname', 'lastname', 'option'];


  // userForm = this.fb.group({
  //   email: ['', [Validators.email, Validators.required]],
  //   firstName: ['', Validators.required],
  //   lastName: ['', Validators.required]
  // });


  constructor(//private readonly fb: FormBuilder,
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
    const dialogRef = this.addDialog.open(DialogAdd, {
      width: '500px',
    });

    dialogRef.afterClosed().subscribe(result => {
      // Ausgabe auf der Konsole zum Debuggen
      //console.log('The dialog was closed');
      // Update der angezeigten User-Liste
      this.GetData();
    });
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

  // public addUser() {
  //   this.usersService.post(this.userForm.value).subscribe(user => {
  //     console.log(user);
  //     this.GetData();
  //   });
  // }

}





/**
 * Komponente fuer den Modal-Dialog zum Hinzufuegen eines Nutzers
 */
@Component({
  selector: 'user.component_dialog_add',
  templateUrl: 'user.component_dialog_add.html',
})
export class DialogAdd {


  userForm = this.fb.group({
    email: ['', [Validators.email, Validators.required]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required]
  });

  // Gruppenberechtigungen
  // TODO: Liste aus Backend laden
  groupPermissionList: permission[] = [
    { value: 'group_adm', viewValue: 'Administrator' },
    { value: 'group_dat', viewValue: 'Datenschutzbeauftragter' },
    { value: 'group_hig', viewValue: 'Hoch' },
    { value: 'group_med', viewValue: 'Normal' },
    { value: 'group_low', viewValue: 'Eingeschränkt' }
  ];
  groupPermissionDefault = ['group_low'];
  groupPermission = new FormControl(this.groupPermissionDefault);

  // Einzelberechtigungen
  // TODO: Liste aus Backend laden
  singlePermissionList: permission[] = [
    { value: 'contact_0', viewValue: 'Anlegen neuer Kontakte' },
    { value: 'contact_1', viewValue: 'Einsehen aller Kontakte' },
    { value: 'contact_2', viewValue: 'Bearbeiten aller Kontakte' },
    { value: 'organization_0', viewValue: 'Anlegen neuer Organisationen' },
    { value: 'organization_1', viewValue: 'Einsehen aller Organisationen' },
    { value: 'organization_2', viewValue: 'Bearbeiten aller Organisationen' }
  ];
  singlePermissionDefault = ['contact_0', 'organization_0'];
  singlePermission = new FormControl(this.singlePermissionDefault);


  constructor(
    private readonly fb: FormBuilder,
    private readonly usersService: UsersService,
    public dialogRef: MatDialogRef<DialogAdd>,
    //@Inject(MAT_DIALOG_DATA) public data: DialogData
  ) { }


  /** 
   * Funktion zum Schliessen des Dialogs
   */
  abortDialog(): void {
    // Schliessen des Dialogs
    this.dialogRef.close();
  }


  /**
   * Funktion zum Speichen des neuesn Users und Schliessen des Dialogs
   */
  public addUser() {
    this.usersService.post(this.userForm.value).subscribe(user => {
      // Ausgabe der Daten auf der Konsole zum Debuggen
      //console.log(user);    
    });

    // TODO: Speichern/Uebermitteln der Gruppenbrerchtigungen
    // Ausgabe der Daten auf der Konsole zum Debuggen
    console.log(this.groupPermission.value);

    // TODO: Speichern/Uebermitteln der Einzelbrerchtigungen
    // Ausgabe der Daten auf der Konsole zum Debuggen
    console.log(this.singlePermission.value);

    // Schliessen des Dialogs
    this.dialogRef.close();
  }

}
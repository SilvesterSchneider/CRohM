import { Component, OnInit, Inject } from '@angular/core';
import { USERS } from './mock-user';
import { FormBuilder, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { UserDto, UsersService, AuthService } from '../../shared/api-generated/api-generated';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';



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
    // Schliessen des Dialogs
    this.dialogRef.close();
  }

}
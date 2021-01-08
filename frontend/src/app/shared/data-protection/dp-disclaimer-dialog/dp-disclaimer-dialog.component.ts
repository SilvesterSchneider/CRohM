import { Component, Inject, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-dp-disclaimer-dialog',
  templateUrl: './dp-disclaimer-dialog.component.html',
  styleUrls: ['./dp-disclaimer-dialog.component.scss']
})
export class DpDisclaimerDialogComponent implements OnInit {

  constructor(
    private readonly dialogRef: MatDialogRef<DpDisclaimerDialogComponent>,
    private readonly router: Router,
    @Inject(MAT_DIALOG_DATA) public permissionToAddRoles: boolean,
    private readonly route: ActivatedRoute) { }

  public ngOnInit(): void {
  }

  public onResolve(){

    this.router.navigate(['settings'], {relativeTo: this.route});
    this.dialogRef.close();
  }
}

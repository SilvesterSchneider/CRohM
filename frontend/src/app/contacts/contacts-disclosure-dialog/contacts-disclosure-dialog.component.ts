import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { ContactService, ContactDto } from '../../shared/api-generated/api-generated';
import { BaseDialogInput } from 'src/app/shared/form/base-dialog-form/base-dialog.component';

@Component({
  selector: 'app-contacts-disclosure-dialog',
  templateUrl: './contacts-disclosure-dialog.component.html',
  styleUrls: ['./contacts-disclosure-dialog.component.scss']
})
export class ContactsDisclosureDialogComponent extends BaseDialogInput<ContactsDisclosureDialogComponent> implements OnInit {
	contact: ContactDto;

  constructor(
		  public dialogRef: MatDialogRef<ContactsDisclosureDialogComponent>,
		  public dialog: MatDialog,
		  @Inject(MAT_DIALOG_DATA) public data: ContactDto,
		  private service: ContactService)
	{
	super(dialogRef, dialog);
	this.contact = data;
	}

	ngOnInit(): void {
	}

	onApprove() {
		this.service.sendDisclosureById(this.contact.id).subscribe(x => this.dialogRef.close());
	}

	onCancel() {
		super.confirmDialog();
	}

	hasChanged(): boolean {
		return false;
	}
}

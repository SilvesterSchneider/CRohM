import { OnInit, Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {
	ContactService,
	HistoryElementCreateDto,
	HistoryElementType
} from 'src/app/shared/api-generated/api-generated';

@Component({
	selector: 'app-contacts-add-history',
	templateUrl: './contacts-add-history.component.html',
	styleUrls: [ './contacts-add-history.component.scss' ]
})
export class ContactsAddHistoryComponent implements OnInit {
	public types: string[] = [ 'E-Mail', 'Telefonanruf', 'Notiz' ];
	public oppoSuitsForm: FormGroup;

	constructor(
		public dialogRef: MatDialogRef<ContactsAddHistoryComponent>,
		@Inject(MAT_DIALOG_DATA) public data: number,
		private fb: FormBuilder,
		private contactService: ContactService
	) {}

	ngOnInit(): void {
		this.oppoSuitsForm = this.fb.group({
			type: [ this.types[0], Validators.required ],
			date: [ '', Validators.required ],
			information: [ '', Validators.required ],
			comment: [ '', Validators.required ]
		});
	}

	saveHistory() {
		let historyToSave: HistoryElementCreateDto;
		let typeToSave = 0;
		const typeText: string = this.oppoSuitsForm.get('type').value;
		if (typeText === this.types[1]) {
			typeToSave = HistoryElementType.PHONE_CALL;
		} else if (typeText === this.types[2]) {
			typeToSave = HistoryElementType.NOTE;
		}
		historyToSave = {
			date: this.oppoSuitsForm.get('date').value,
			name: this.oppoSuitsForm.get('information').value,
			type: typeToSave,
			comment: this.oppoSuitsForm.get('comment').value
		};
		this.contactService.postHistoryElement(historyToSave, this.data).subscribe((x) => this.dialogRef.close());
	}

	close() {
		this.dialogRef.close();
	}
}

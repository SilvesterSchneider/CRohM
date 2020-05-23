import { OnInit, Component, Inject, NgModule } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ContactService, HistoryElementCreateDto, HistoryElementType } from 'src/app/shared/api-generated/api-generated';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'contacts-add-history',
    templateUrl: './contacts-add-history.component.html',
    styleUrls: ['./contacts-add-history.component.scss']
  })

export class ContactsAddHistoryComponent implements OnInit {
    public types: string[] = ['E-Mail', 'Telefonanruf', 'Notiz'];
    public oppoSuitsForm: FormGroup;
    constructor(
        public dialogRef: MatDialogRef<ContactsAddHistoryComponent>,
        @Inject(MAT_DIALOG_DATA) public data: number,
        private contactService: ContactService,
        private fb: FormBuilder) {}
        
    ngOnInit(): void {
        this.oppoSuitsForm = this.fb.group({
            type: [this.types[0], Validators.required],
            date: ['', Validators.required],
            information: ['', Validators.required]
          })
    }

    saveHistory() {
        let historyToSave: HistoryElementCreateDto;
        let type = 0;
        const typeText: string = this.oppoSuitsForm.get('type').value;
        if (typeText == this.types[1]) {
            type = HistoryElementType.PHONE_CALL;
        } else if (typeText == this.types[2]) {
            type = HistoryElementType.NOTE;
        }
        historyToSave = {
            date: this.oppoSuitsForm.get('date').value,
            name: this.oppoSuitsForm.get('information').value,
            type: type
        };
        
        this.contactService.postHistoryElement(historyToSave, this.data).subscribe(x => this.dialogRef.close());
    }
}
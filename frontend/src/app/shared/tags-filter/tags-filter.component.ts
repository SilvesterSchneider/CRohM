import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ContactDto, OrganizationDto, TagDto } from '../api-generated/api-generated';

@Component({
  selector: 'app-tags-filter',
  templateUrl: './tags-filter.component.html',
  styleUrls: ['./tags-filter.component.scss']
})

export class TagsFilterComponent implements OnInit {
  @ViewChild('tagInput') tagInput: ElementRef<HTMLInputElement>;
	tagsControl = new FormControl();
	selectedTags: TagDto[] = new Array<TagDto>();
	separatorKeysCodes: number[] = [ENTER, COMMA];
  filteredTagsObservable: Observable<string[]>;
  allPredefinedTags: string[] = [ 'Lehrbeauftragter', 'Kunde', 'Politiker', 'Firma', 'Behörde', 'Bildungseinrichtung', 'Institute', 'Ministerium',
  'Emeriti', 'Alumni'];
	allTags: string[] = this.allPredefinedTags;
	removable = true;
  selectable = true;
  refreshFunction: () => void;

  constructor() {
    this.filteredTagsObservable = this.tagsControl.valueChanges.pipe(
			map((tag: string | null) => tag ? this._filter(tag) : this.allTags.slice()));
  }

  ngOnInit(): void {
  }

  private _filter(value: string): string[] {
		const tagValue = value.toLowerCase();

		return this.allTags.filter(tag => tag.toLowerCase().indexOf(tagValue) === 0);
	  }

	addTag(event: Event) {
		const value = (event.target as HTMLInputElement).value;
		if (value.length > 0 && this.selectedTags.find(a => a.name === value) == null) {
      this.selectedTags.push({
				id: 0,
				name: value
      });
      if (this.refreshFunction != null) {
        this.refreshFunction();
      }
		}
		this.tagsControl.setValue('');
	}

	removeTag() {
		if (this.selectedTags.length > 0) {
      this.selectedTags.splice(this.selectedTags.length - 1, 1);
      if (this.refreshFunction != null) {
        this.refreshFunction();
      }
		}
	}

	remove(tag: TagDto) {
		const index = this.selectedTags.indexOf(tag);
		if (index >= 0) {
      this.selectedTags.splice(index, 1);
      if (this.refreshFunction != null) {
        this.refreshFunction();
      }
		}
	}

	selected(event: MatAutocompleteSelectedEvent): void {
		if (this.selectedTags.find(a => a.name === event.option.viewValue) == null) {
      this.selectedTags.push({
			  id: 0,
			  name: event.option.viewValue
      });
      this.tagInput.nativeElement.value = '';
      this.tagsControl.setValue(null);
      if (this.refreshFunction != null) {
        this.refreshFunction();
      }
    }
  }

  public areAllTagsIncluded(valuesToCheck: TagDto[]): boolean {
    if (this.selectedTags.length === 0) {
      return true;
    }
    let found = true;
    this.selectedTags.forEach(x => {
      if (valuesToCheck.find(y => y.name === x.name) == null) {
        found = false;
      }
    });
    return found;
  }

  public setRefreshTableFunction(functionToCall: () => void) {
    this.refreshFunction = functionToCall;
  }

  public updateTagsInAutofill(data: any[]) {
    if (data == null || data.length === 0) {
      return;
    }
    this.resetTagsFromAutoFill();
    const listOfNewTags: string[] = new Array<string>();
    data.forEach(x => {
      if (x.tags != null && x.tags.length > 0) {
        this.checkTagsToInsert(x.tags, listOfNewTags);
      }
    });
    this.addTagsToAutoFill(listOfNewTags);
  }

  private checkTagsToInsert(tags: TagDto[], listOfNewTags: string[]) {
    tags.forEach(x => {
      if (listOfNewTags.find(y => y === x.name) == null && this.allPredefinedTags.find(z => z === x.name) == null) {
        listOfNewTags.push(x.name);
      }
    });
  }

  private addTagsToAutoFill(tagsToAdd: string[]) {
    tagsToAdd.forEach(x => this.allTags.push(x));
  }

  private resetTagsFromAutoFill() {
    this.allTags = this.allPredefinedTags;
  }
}

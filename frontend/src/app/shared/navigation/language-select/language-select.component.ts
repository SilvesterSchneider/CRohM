import { Component } from '@angular/core';
import { TranslationService } from '../../translation/translation.service';

@Component({
  selector: 'app-language-select',
  templateUrl: './language-select.component.html',
  styleUrls: ['./language-select.component.scss']
})
export class LanguageSelectComponent {
  constructor(public readonly translationService: TranslationService) { }

  public getAllLanguages() {
    return TranslationService.LANGUAGES;
  }
}

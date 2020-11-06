import { Component } from '@angular/core';
import { TranslationService } from './shared/translation/translation.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'crms-frontend';

  constructor(private translationService: TranslationService) {
    translationService.setDefaultLanguage();
  }
}

import { LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './shared/material.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ContactsModule } from './contacts/contacts.module';
import { LoginComponent } from './login/login.component';
import { JwtModule } from '@auth0/angular-jwt';
import { JwtService } from './shared/jwt.service';
import { UserMenuComponent } from './shared/navigation/user-menu/user-menu.component';
import { SidenavComponent } from './shared/navigation/sidenav/sidenav.component';
import { OrganizationsModule } from './organizations/organizations.module';
import { EventsModule } from './events/events.module';
import { ProgressSpinnerInterceptor } from './shared/progress-spinner/progress-spinner.interceptor';
import { OverlayModule } from '@angular/cdk/overlay';
import { ConfirmDialogComponent } from './shared/form/confirmdialog/confirmdialog.component';
import { ChangePasswordComponent } from './login/change-password-dialog/change-password-dialog.component';
import { StatisticsModule } from './statistics/statistics.module';
import { LanguageSelectComponent } from './shared/navigation/language-select/language-select.component';
import { MissingTranslationHandler, TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { MissingTranslationLogger } from './shared/translation/missing-translation-logger';
import { TranslationService } from './shared/translation/translation.service';
import localeDe from '@angular/common/locales/de';
import { registerLocaleData } from '@angular/common';
import { MAT_DATE_LOCALE } from '@angular/material/core';

registerLocaleData(localeDe);

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    UserMenuComponent,
    SidenavComponent,
    ConfirmDialogComponent,
    ChangePasswordComponent,
    LanguageSelectComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MaterialModule,
    OverlayModule,
    FlexLayoutModule,
    ContactsModule,
    StatisticsModule,
    EventsModule,
    OrganizationsModule,
    // Configure where JWT is stored / read from
    JwtModule.forRoot({
      config: {
        tokenGetter: () => localStorage.getItem(JwtService.LS_KEY)
      }
    }),
    // Configure Translation Module
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      },
      missingTranslationHandler: { provide: MissingTranslationHandler, useClass: MissingTranslationLogger },
    })
  ],
  providers: [
    // Use ProgressSpinnerInterceptor
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ProgressSpinnerInterceptor,
      multi: true,
    },
    // Provide locale using translationService
    {
      provide: LOCALE_ID,
      deps: [TranslationService],
      useFactory: (translationService) => translationService.getLocale()
    },
    {
      provide: MAT_DATE_LOCALE,
      deps: [TranslationService],
      useFactory: (translationService) => translationService.getLocale()
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

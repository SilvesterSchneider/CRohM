import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
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
import { ConfirmDialogComponent } from './confirmdialog/confirmdialog.component';
import { EventsModule } from './events/events.module';
import { ProgressSpinnerInterceptor } from './shared/progress-spinner/progress-spinner.interceptor';
import { OverlayModule } from '@angular/cdk/overlay';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    UserMenuComponent,
    SidenavComponent,
    ConfirmDialogComponent
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
    EventsModule,
    OrganizationsModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: () => localStorage.getItem(JwtService.LS_KEY)
      }
    })
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ProgressSpinnerInterceptor,
      multi: true,
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

/// <reference types="@angular/localize" />

import { withInterceptorsFromDi, provideHttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { AppRoutingModule } from './app/app-routing.module';
import { AppComponent } from './app/app.component';
import { APP_INITIALIZER, importProvidersFrom } from '@angular/core';
import { AuthenticationService } from './app/services/authentication.service';
import { firstValueFrom } from 'rxjs';
import { AuthInterceptor } from './app/interceptors/auth.interseptor';

export function initializeApp(authService: AuthenticationService): () => Promise<void> {
  return async () => {
    try {
      const session = await firstValueFrom(authService.getSession());
      console.log("session", session);
      if (!session.isAuthenticated && !session.isError) {
        console.log("user not logged in");
        window.location.href = '/login';
      }
    } catch (error) {
      console.error('Error getting session:', error);
      window.location.href = '/error.html';
    }
  };
}

bootstrapApplication(AppComponent, {
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [AuthenticationService],
      multi: true,
    },

    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    importProvidersFrom(BrowserModule, AppRoutingModule),
    provideHttpClient(withInterceptorsFromDi())
    ]
})
  .catch(err => console.error(err));

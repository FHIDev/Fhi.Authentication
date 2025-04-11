/// <reference types="@angular/localize" />

import { withInterceptorsFromDi, provideHttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { routes } from './app/app.routes';
import { AppComponent } from './app/app.component';
import { APP_INITIALIZER, importProvidersFrom, inject } from '@angular/core';
import { AuthenticationService } from './app/services/authentication.service';
import { firstValueFrom } from 'rxjs';
import { AuthInterceptor } from './app/interceptors/auth.interseptor';
import { provideRouter } from '@angular/router';

export function initializeApp(): () => Promise<void> {
  const authService = inject(AuthenticationService);
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
      //window.location.href = '/error.html';
    }
  };
}

bootstrapApplication(AppComponent, {
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      multi: true,
    },

    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    provideRouter(routes),
    importProvidersFrom(BrowserModule),
    provideHttpClient(withInterceptorsFromDi())
    ]
})
  .catch(err => console.error(err));

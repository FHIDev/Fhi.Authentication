import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { catchError, shareReplay, Observable } from 'rxjs';
import { of } from 'rxjs';

const CACHE_SIZE = 1;

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private readonly http = inject(HttpClient);
  private session$: Observable<Session> | null = null;

  public getSession(ignoreCache: boolean = false): Observable<Session> {
    if (!this.session$ || ignoreCache) {
      this.session$ = this.http.get<Session>('/session', { withCredentials: true }).pipe(
        catchError((error) => {
          console.log(error);
          return of({ isAuthenticated: false, isError: true });
        })
      );
    }
    return this.session$;
  }
}

//  //initSession(done: () => void): void {
  //  //  this.http.get('/api/v1/user-session', { observe: 'response' })
  //  //    .subscribe({
  //  //      next: () => {
  //  //        this.authenticated = true;
  //  //        done(); 
  //  //      },
  //  //      error: err => {
  //  //        if (err.status === 401) {
  //  //          //Hard redirect causeing full reload to avoid CORS
  //  //          window.location.href = '/login';
  //  //          return;
  //  //        }
  //  //        done(); 
  //  //      }
  //  //    });
  //  //}
    

type Session = {
  isAuthenticated: boolean | false,
  isError: boolean | false
  
}


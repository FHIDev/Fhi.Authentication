import { Routes } from '@angular/router';
import { UserSessionComponent } from './user-token/user-token.component';
import { WeatherForecastComponent } from './weather/weather.component';
import { HomeComponent } from './home/home.component';

export const routes: Routes = [
  { 
    path: '', 
    component: HomeComponent 
  },
  { 
    path: 'tokens', 
    component: UserSessionComponent 
  },
  { 
    path: 'weather', 
    component: WeatherForecastComponent 
  }
];

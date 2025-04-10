import { Component } from '@angular/core';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { WeatherForecastComponent } from "./weather/weather.component";
import { UserSessionComponent } from './user-session/user-session.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    standalone: true,
    imports: [NavMenuComponent, WeatherForecastComponent, UserSessionComponent]
})
export class AppComponent {

  title = 'Angular.Client';
}

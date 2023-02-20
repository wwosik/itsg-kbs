import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LogLevel } from './logging.service';

@Injectable({
	providedIn: 'root',
})
export class ConfigService {
	public readonly current$ = new BehaviorSubject<Config>({
		logLevelForBackend: LogLevel.debug,
		logLevelForConsole: LogLevel.debug,
	});

	constructor(private readonly http: HttpClient) {}

	load$(): Observable<Config> {
		return this.http.get<Config>('/api/config').pipe(tap(val => this.current$.next(val)));
	}
}

export interface Config {
	logLevelForBackend: LogLevel;
	logLevelForConsole: LogLevel;
}

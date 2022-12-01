clear all; clc;
format compact;

%Import data from .edf file into a timetable
data = edfread('chb01_03.edf');

fs = 256; % sampling frequncy for database
signal = [];

% Chosen EEG channels
O1 = 8;
O2 = 12;
T3 = 20;
T4 = 22;

totalSumLyap = 0;
Tstart = 2100; % User enter: minute mark on database * 60 to get seconds
Tend = Tstart+5; 
Range = Tend-Tstart; % selected data being tested for a seizure
for part_i = 1:63 % iterations
    totalsig = zeros(1537,4); 
     for channel = [O1 O2 T3 T4]
        channelSumLyap = 0;
        signal = 0;
        for T = (Tstart+(5*part_i)):(Tend+(5*part_i)) % 5 is for the seconds between iterations
            chunk = [cell2mat(data{T,channel})];
            signal = [signal; chunk];
        end
        totalsig(:,1) = [];
        totalsig = [totalsig signal];
        %split the signal into half second chunks
        [XR, eLag, eDim] = phaseSpaceReconstruction(signal);
        CHLyap = lyapunovExponent(signal,256,eLag,4);
     end
     AvgChLLE(part_i) = CHLyap / (4);
end
AvgChLLE

%eRange = fs*Range - 10;
%lyapunovExponent(signal,fs,eLag,eDim,'ExpansionRange',eRange)
lyap = lyapunovExponent(totalsig,fs,eLag,4)

% Plotting used for presentation

% Plotting of the LLE
%xaxis = linspace(Tstart,T,iterations); 
%plot(xaxis,AvgChLLE)

%Plotting bar graph on top of LLE
%bar = zeros(0:iterations);
%bar(value:value) = a height
%plot(xaxis,AvgChLLE); hold on; plot(xaxis,bar)